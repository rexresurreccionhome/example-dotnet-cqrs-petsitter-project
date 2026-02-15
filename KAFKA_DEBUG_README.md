# Kafka Debugging Guide for CQRS Pet Sitter Project

This guide provides comprehensive debugging tools and commands for working with Kafka in your CQRS Pet Sitter project using Docker.

## 📋 Table of Contents

- [Overview](#overview)
- [Project-Specific Topics](#project-specific-topics)
- [Docker Container Access](#docker-container-access)
- [Topic Management](#topic-management)
- [Consumer Group Management](#consumer-group-management)
- [Message Inspection](#message-inspection)
- [Stream Purging](#stream-purging)
- [Debugging Event Flow](#debugging-event-flow)
- [Performance Monitoring](#performance-monitoring)
- [Kafka Commands Cheatsheet](#kafka-commands-cheatsheet)
- [Environment Variables Setup](#environment-variables-setup)
- [Troubleshooting](#troubleshooting)

---

## 🔧 Environment Variables Setup

Your CQRS implementation relies on environment variables to determine which Kafka topics to use for event publishing and consuming.

### Required Environment Variables

| Variable Name | Purpose | Default Value | Used By |
|---------------|---------|---------------|---------|
| `KAFKA_TOPIC_PETSITTER_PET_EVENTS` | Pet aggregate events topic | `PetSitterPetEvents` | Command & Query Pet services |
| `KAFKA_TOPIC_PETSITTER_SITTER_EVENTS` | Sitter aggregate events topic | `PetSitterSitterEvents` | Command & Query Sitter services |

### Development Environment Setup

#### Method 1: launchSettings.json (Recommended for Development)
Your project already has these configured in the `Properties/launchSettings.json` files:

**Pet Services** (`Command/Pet/Command.Pet.Api/Properties/launchSettings.json` & `Query/Pet/Query.Pet.Api/Properties/launchSettings.json`):
```json
{
  "profiles": {
    "http": {
      "environmentVariables": {
        "KAFKA_TOPIC_PETSITTER_PET_EVENTS": "PetSitterPetEvents"
      }
    },
    "https": {
      "environmentVariables": {
        "KAFKA_TOPIC_PETSITTER_PET_EVENTS": "PetSitterPetEvents"
      }
    }
  }
}
```

**Sitter Services** (`Command/Sitter/Command.Sitter.Api/Properties/launchSettings.json` & `Query/Sitter/Query.Sitter.Api/Properties/launchSettings.json`):
```json
{
  "profiles": {
    "http": {
      "environmentVariables": {
        "KAFKA_TOPIC_PETSITTER_SITTER_EVENTS": "PetSitterSitterEvents"
      }
    },
    "https": {
      "environmentVariables": {
        "KAFKA_TOPIC_PETSITTER_SITTER_EVENTS": "PetSitterSitterEvents"
      }
    }
  }
}
```

#### Method 2: System Environment Variables
Set system-wide environment variables:

**Windows (PowerShell)**:
```powershell
[Environment]::SetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_PET_EVENTS", "PetSitterPetEvents", "User")
[Environment]::SetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_SITTER_EVENTS", "PetSitterSitterEvents", "User")
```

**macOS/Linux (Bash/Zsh)**:
```bash
export KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents
export KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents

# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents' >> ~/.zshrc
echo 'export KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents' >> ~/.zshrc
```

#### Method 3: .env File (Optional)
Create a `.env` file in your project root:
```env
KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents
KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents
```

### Docker Compose Environment Setup

Add environment variables to your `compose.yaml` file for containerized applications:

```yaml
services:
  pet-command-api:
    build: ./Command/Pet/Command.Pet.Api
    environment:
      - KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents
    networks:
      - petsitter_net

  pet-query-api:
    build: ./Query/Pet/Query.Pet.Api
    environment:
      - KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents
    networks:
      - petsitter_net

  sitter-command-api:
    build: ./Command/Sitter/Command.Sitter.Api
    environment:
      - KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents
    networks:
      - petsitter_net

  sitter-query-api:
    build: ./Query/Sitter/Query.Sitter.Api
    environment:
      - KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents
    networks:
      - petsitter_net
```

### Production Environment Setup

#### Method 1: Container Orchestration
**Kubernetes**:
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: pet-command-api
spec:
  template:
    spec:
      containers:
      - name: pet-command-api
        image: your-registry/pet-command-api:latest
        env:
        - name: KAFKA_TOPIC_PETSITTER_PET_EVENTS
          value: "PetSitterPetEvents"
```

**Docker Swarm**:
```yaml
version: '3.8'
services:
  pet-command-api:
    image: your-registry/pet-command-api:latest
    environment:
      - KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents
```

#### Method 2: Configuration Management
Use configuration management tools like:
- **Azure App Configuration**
- **AWS Parameter Store** 
- **HashiCorp Vault**
- **Kubernetes ConfigMaps/Secrets**

### Environment-Specific Topic Names

You can customize topic names per environment:

| Environment | Pet Events Topic | Sitter Events Topic |
|-------------|------------------|-------------------|
| Development | `PetSitterPetEvents` | `PetSitterSitterEvents` |
| Staging | `Staging-PetSitterPetEvents` | `Staging-PetSitterSitterEvents` |
| Production | `Prod-PetSitterPetEvents` | `Prod-PetSitterSitterEvents` |

### Verifying Environment Variable Setup

#### Check in Application
```bash
# Run your application and check logs for successful topic configuration
dotnet run --project Command/Pet/Command.Pet.Api

# Look for logs indicating successful Kafka connection
```

#### Test Environment Variables
Create a simple test to verify environment variables:

```csharp
// Add to your Program.cs for testing
var petEventsTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_PET_EVENTS");
var sitterEventsTopic = Environment.GetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_SITTER_EVENTS");

Console.WriteLine($"Pet Events Topic: {petEventsTopic ?? "NOT SET"}");
Console.WriteLine($"Sitter Events Topic: {sitterEventsTopic ?? "NOT SET"}");
```

#### Verify Topics in Kafka
```bash
# Check if your configured topics exist
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

# Should show:
# PetSitterPetEvents
# PetSitterSitterEvents
```

### Common Configuration Issues

#### Issue 1: Environment Variable Not Set
**Error**: `Environment variable KAFKA_TOPIC_PETSITTER_*_EVENTS is empty`

**Solution**: Verify the environment variable is set in your launch configuration or system environment.

#### Issue 2: Topic Name Mismatch
**Error**: Events published but not consumed

**Solution**: Ensure both Command and Query sides use the same topic name.

#### Issue 3: Case Sensitivity
**Error**: Topic not found

**Solution**: Environment variable names are case-sensitive. Use exact names:
- ✅ `KAFKA_TOPIC_PETSITTER_PET_EVENTS`
- ❌ `kafka_topic_petsitter_pet_events`

### Testing Different Topic Configurations

#### Integration Testing
Use different topic names for testing to avoid conflicts:

```csharp
// In your test setup (Command.Pet.Api.Tests.Integration/PetApiWebApplicationFactory.cs)
Environment.SetEnvironmentVariable("KAFKA_TOPIC_PETSITTER_PET_EVENTS", "TestPetSitterPetEvents");
```

#### Load Testing
Use dedicated topics for load testing:
```bash
export KAFKA_TOPIC_PETSITTER_PET_EVENTS=LoadTest-PetSitterPetEvents
export KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=LoadTest-PetSitterSitterEvents
```

---

## 📝 Overview

Your CQRS implementation uses Kafka for event streaming between Command and Query sides:

### Architecture Flow:
1. **Command Side** → Publishes events to Kafka topics
2. **Query Side** → Consumes events from Kafka topics to update read models

### Docker Setup:
- **Kafka Container**: `kafka` (bitnami/kafka:3.5)
- **Zookeeper Container**: `zookeeper` (bitnami/zookeeper:3.9)
- **Network**: `petsitter_net`
- **Kafka Port**: `localhost:9092`

---

## 🏷️ Project-Specific Topics

Your implementation uses these Kafka topics:

| Topic Name | Environment Variable | Purpose |
|------------|---------------------|---------|
| `PetSitterPetEvents` | `KAFKA_TOPIC_PETSITTER_PET_EVENTS` | Pet aggregate events |
| `PetSitterSitterEvents` | `KAFKA_TOPIC_PETSITTER_SITTER_EVENTS` | Sitter aggregate events |

### Event Types Published:
**Pet Events:**
- `PetCreatedEvent`
- `PetUpdatedEvent`
- `PetDeletedEvent`

**Sitter Events:**
- `SitterCreatedEvent`
- `SitterUpdatedEvent`
- `SitterDeletedEvent`

---

## 🐳 Docker Container Access

### Access Kafka Container Shell
```bash
# Enter Kafka container
docker exec -it kafka /bin/bash

# Alternative: Run single command
docker exec -it kafka kafka-topics.sh --list --bootstrap-server localhost:9092
```

### Check Container Status
```bash
# List all containers
docker ps

# Check specific containers
docker ps --filter "name=kafka"
docker ps --filter "name=zookeeper"

# Check container logs
docker logs kafka
docker logs zookeeper
```

---

## 📊 Topic Management

### List All Topics
```bash
# From host machine
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

# From inside Kafka container
kafka-topics.sh --list --bootstrap-server localhost:9092
```

### Check Topic Details
```bash
# Describe specific topic
docker exec kafka kafka-topics.sh --describe --topic PetSitterPetEvents --bootstrap-server localhost:9092
docker exec kafka kafka-topics.sh --describe --topic PetSitterSitterEvents --bootstrap-server localhost:9092

# Check all topics
docker exec kafka kafka-topics.sh --describe --bootstrap-server localhost:9092
```

### Create Topics Manually (if needed)
```bash
# Create Pet events topic
docker exec kafka kafka-topics.sh --create --topic PetSitterPetEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1

# Create Sitter events topic
docker exec kafka kafka-topics.sh --create --topic PetSitterSitterEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

### Delete Topics
```bash
# Delete specific topic
docker exec kafka kafka-topics.sh --delete --topic PetSitterPetEvents --bootstrap-server localhost:9092
docker exec kafka kafka-topics.sh --delete --topic PetSitterSitterEvents --bootstrap-server localhost:9092
```

---

## 👥 Consumer Group Management

### List Consumer Groups
```bash
# List all consumer groups
docker exec kafka kafka-consumer-groups.sh --list --bootstrap-server localhost:9092
```

### Check Consumer Group Status
```bash
# Check specific consumer group
docker exec kafka kafka-consumer-groups.sh --describe --group [GROUP_NAME] --bootstrap-server localhost:9092

# Check all consumer groups
docker exec kafka kafka-consumer-groups.sh --describe --all-groups --bootstrap-server localhost:9092
```

### Consumer Group Lag Monitoring
```bash
# Monitor lag for specific group
docker exec kafka kafka-consumer-groups.sh --describe --group [GROUP_NAME] --bootstrap-server localhost:9092 --offsets

# Real-time lag monitoring
watch 'docker exec kafka kafka-consumer-groups.sh --describe --group [GROUP_NAME] --bootstrap-server localhost:9092 --offsets'
```

---

## 🔍 Message Inspection

### Consume Messages from Beginning
```bash
# Read all Pet events
docker exec kafka kafka-console-consumer.sh --topic PetSitterPetEvents --from-beginning --bootstrap-server localhost:9092

# Read all Sitter events
docker exec kafka kafka-console-consumer.sh --topic PetSitterSitterEvents --from-beginning --bootstrap-server localhost:9092
```

### Consume Latest Messages
```bash
# Read latest Pet events (real-time)
docker exec kafka kafka-console-consumer.sh --topic PetSitterPetEvents --bootstrap-server localhost:9092

# Read latest Sitter events (real-time)
docker exec kafka kafka-console-consumer.sh --topic PetSitterSitterEvents --bootstrap-server localhost:9092
```

### Consume with Message Details
```bash
# Show message key, value, timestamp, and partition
docker exec kafka kafka-console-consumer.sh \
  --topic PetSitterPetEvents \
  --from-beginning \
  --bootstrap-server localhost:9092 \
  --property print.key=true \
  --property print.value=true \
  --property print.timestamp=true \
  --property print.partition=true \
  --property print.offset=true
```

### Consume Specific Number of Messages
```bash
# Read only last 10 messages
docker exec kafka kafka-console-consumer.sh \
  --topic PetSitterPetEvents \
  --bootstrap-server localhost:9092 \
  --max-messages 10
```

---

## 🗑️ Stream Purging

### Method 1: Reset Consumer Group Offsets
```bash
# Reset specific consumer group to earliest
docker exec kafka kafka-consumer-groups.sh \
  --reset-offsets \
  --group [CONSUMER_GROUP] \
  --topic PetSitterPetEvents \
  --to-earliest \
  --bootstrap-server localhost:9092 \
  --execute

# Reset to latest (skip all existing messages)
docker exec kafka kafka-consumer-groups.sh \
  --reset-offsets \
  --group [CONSUMER_GROUP] \
  --topic PetSitterPetEvents \
  --to-latest \
  --bootstrap-server localhost:9092 \
  --execute
```

### Method 2: Delete and Recreate Topics
```bash
# Delete topic
docker exec kafka kafka-topics.sh --delete --topic PetSitterPetEvents --bootstrap-server localhost:9092

# Wait a moment, then recreate
docker exec kafka kafka-topics.sh --create --topic PetSitterPetEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

### Method 3: Set Retention Policy (Temporary)
```bash
# Set very short retention (1 second)
docker exec kafka kafka-configs.sh \
  --alter \
  --entity-type topics \
  --entity-name PetSitterPetEvents \
  --add-config retention.ms=1000 \
  --bootstrap-server localhost:9092

# Wait for cleanup, then restore retention
docker exec kafka kafka-configs.sh \
  --alter \
  --entity-type topics \
  --entity-name PetSitterPetEvents \
  --add-config retention.ms=604800000 \
  --bootstrap-server localhost:9092
```

---

## 🐛 Debugging Event Flow

### Check Event Publishing (Command Side)
```bash
# Monitor events being published in real-time
docker exec kafka kafka-console-consumer.sh \
  --topic PetSitterPetEvents \
  --bootstrap-server localhost:9092 \
  --property print.timestamp=true

# Test event publishing by triggering API calls
curl -X POST http://localhost:[COMMAND_API_PORT]/api/pet \
  -H "Content-Type: application/json" \
  -d '{"name":"TestPet","memberId":"123e4567-e89b-12d3-a456-426614174000"}'
```

### Check Event Consumption (Query Side)
```bash
# Monitor Query side consumer logs
docker logs -f [QUERY_CONTAINER_NAME]

# Check consumer group status
docker exec kafka kafka-consumer-groups.sh \
  --describe \
  --group [QUERY_CONSUMER_GROUP] \
  --bootstrap-server localhost:9092
```

### Verify Event Structure
Your events should have this JSON structure:
```json
{
  "Id": "guid",
  "Version": 1,
  "AggregateId": "guid",
  "TimeStamp": "2024-01-01T00:00:00Z",
  // Event-specific properties
}
```

---

## ⚡ Performance Monitoring

### Monitor Topic Throughput
```bash
# Monitor messages per second
docker exec kafka kafka-run-class.sh kafka.tools.ConsumerPerformance \
  --topic PetSitterPetEvents \
  --bootstrap-server localhost:9092 \
  --messages 1000
```

### Check Broker Resource Usage
```bash
# Container resource usage
docker stats kafka zookeeper

# Disk usage in container
docker exec kafka df -h
```

### Monitor Log Segments
```bash
# Check topic log directory
docker exec kafka ls -la /bitnami/kafka/data/

# Check specific topic logs
docker exec kafka find /bitnami/kafka/data -name "*PetSitterPetEvents*" -type f
```

---

## 🛠️ Kafka Commands Cheatsheet

### Essential Docker + Kafka Commands
```bash
# Container Management
docker-compose up -d kafka zookeeper              # Start Kafka services
docker-compose down                                # Stop all services
docker exec -it kafka /bin/bash                   # Enter Kafka container

# Topic Operations
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092
docker exec kafka kafka-topics.sh --describe --topic [TOPIC] --bootstrap-server localhost:9092
docker exec kafka kafka-topics.sh --create --topic [TOPIC] --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
docker exec kafka kafka-topics.sh --delete --topic [TOPIC] --bootstrap-server localhost:9092

# Consumer Operations
docker exec kafka kafka-console-consumer.sh --topic [TOPIC] --bootstrap-server localhost:9092 --from-beginning
docker exec kafka kafka-consumer-groups.sh --list --bootstrap-server localhost:9092
docker exec kafka kafka-consumer-groups.sh --describe --group [GROUP] --bootstrap-server localhost:9092

# Producer Operations (for testing)
docker exec -it kafka kafka-console-producer.sh --topic [TOPIC] --bootstrap-server localhost:9092

# Configuration Management
docker exec kafka kafka-configs.sh --describe --entity-type topics --entity-name [TOPIC] --bootstrap-server localhost:9092
docker exec kafka kafka-configs.sh --alter --entity-type topics --entity-name [TOPIC] --add-config retention.ms=86400000 --bootstrap-server localhost:9092

# Monitoring
docker logs kafka                                  # Kafka logs
docker logs zookeeper                             # Zookeeper logs
docker stats kafka zookeeper                     # Resource usage
```

### Project-Specific Quick Commands
```bash
# List your project topics
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092 | grep PetSitter

# Monitor Pet events in real-time
docker exec kafka kafka-console-consumer.sh --topic PetSitterPetEvents --bootstrap-server localhost:9092

# Monitor Sitter events in real-time  
docker exec kafka kafka-console-consumer.sh --topic PetSitterSitterEvents --bootstrap-server localhost:9092

# Check all Pet events from beginning with details
docker exec kafka kafka-console-consumer.sh --topic PetSitterPetEvents --from-beginning --bootstrap-server localhost:9092 --property print.key=true --property print.timestamp=true

# Reset consumer for Query side (replace [GROUP] with actual group name)
docker exec kafka kafka-consumer-groups.sh --reset-offsets --group [GROUP] --topic PetSitterPetEvents --to-latest --bootstrap-server localhost:9092 --execute
```

---

## 🚨 Troubleshooting

### Common Issues and Solutions

#### 1. Unknown Topic or Partition Error
**Error**: `ConsumeException: 'Subscribed topic not available: PetSitterPetEvents: Broker: Unknown topic or partition'`

This error occurs when your Query side consumer tries to subscribe to a Kafka topic that doesn't exist yet.

**Root Cause**: 
- The Query consumer starts before any Command side has published events
- Topics are only auto-created when a producer first publishes to them (if auto-create is enabled)
- Consumers cannot auto-create topics when subscribing

**Debugging Steps**:

1. **Check if topics exist**:
```bash
# List all existing topics
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092

# Should show your project topics:
# PetSitterPetEvents
# PetSitterSitterEvents
```

2. **Check topic details** (if topics exist):
```bash
# Get topic configuration
docker exec kafka kafka-topics.sh --describe --topic PetSitterPetEvents --bootstrap-server localhost:9092
docker exec kafka kafka-topics.sh --describe --topic PetSitterSitterEvents --bootstrap-server localhost:9092
```

3. **Verify Kafka container is running**:
```bash
# Check container status
docker ps | grep kafka

# Check Kafka logs for errors
docker logs kafka
```

**Solutions**:

**Method 1: Create Topics Manually (Immediate Fix)**:
```bash
# Create Pet events topic
docker exec kafka kafka-topics.sh --create --topic PetSitterPetEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1

# Create Sitter events topic
docker exec kafka kafka-topics.sh --create --topic PetSitterSitterEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1

# Verify topics were created
docker exec kafka kafka-topics.sh --list --bootstrap-server localhost:9092
```

**Method 2: Publish a Test Event from Command Side**:
```bash
# Start Command API first, then make a test request
curl -X POST http://localhost:[COMMAND_PORT]/api/pet \
  -H "Content-Type: application/json" \
  -d '{"name":"TestPet","memberId":"123e4567-e89b-12d3-a456-426614174000"}'

# This will auto-create the topic when the event is published
```

**Method 3: Add Startup Topic Creation**:
Add this to your `compose.yaml` to create topics on startup:

```yaml
services:
  kafka-setup:
    image: docker.io/bitnami/kafka:3.5
    depends_on:
      - kafka
    command: >
      bash -c "
        sleep 10 &&
        kafka-topics.sh --create --if-not-exists --topic PetSitterPetEvents --bootstrap-server kafka:9092 --partitions 3 --replication-factor 1 &&
        kafka-topics.sh --create --if-not-exists --topic PetSitterSitterEvents --bootstrap-server kafka:9092 --partitions 3 --replication-factor 1
      "
    networks:
      - petsitter_net
```

**Prevention**: Start services in this order:
1. Kafka & Zookeeper
2. Command APIs (to create topics when first events are published)
3. Query APIs (to consume from existing topics)

#### 2. Topics Not Found (Alternative Scenarios)
**Error**: `Topic 'PetSitterPetEvents' not found`

**Solutions**:
```bash
# Check if auto-create is enabled (should be true in your setup)
docker exec kafka kafka-configs.sh --describe --entity-type brokers --entity-name 0 --bootstrap-server localhost:9092 | grep auto.create

# Manually create topic if needed
docker exec kafka kafka-topics.sh --create --topic PetSitterPetEvents --bootstrap-server localhost:9092 --partitions 3 --replication-factor 1
```

#### 3. Consumer Startup Order Issues
**Issue**: Query side consumers fail because Command side hasn't created topics yet

**Debug & Solution**:
```bash
# Check service startup order
docker-compose logs --timestamps | grep -E "(kafka|query|command)"

# Solution: Add topic initialization service or start Command APIs first
# Or manually create topics as shown in issue #1 above
```

#### 4. Consumer Group Lag
**Issue**: Events not being processed by Query side

**Debug Steps**:
```bash
# Check consumer group status
docker exec kafka kafka-consumer-groups.sh --describe --group [YOUR_CONSUMER_GROUP] --bootstrap-server localhost:9092

# Check Query side application logs
docker logs [QUERY_CONTAINER_NAME]

# Restart Query side consumer
docker restart [QUERY_CONTAINER_NAME]
```

#### 5. Connection Issues
**Error**: `Connection refused` or timeout errors

**Solutions**:
```bash
# Check if containers are running
docker ps | grep -E "(kafka|zookeeper)"

# Check network connectivity
docker exec kafka nc -zv localhost 9092

# Restart Kafka services
docker-compose restart kafka zookeeper
```

#### 6. Out of Sync Consumer
**Issue**: Query side not processing latest events

**Solutions**:
```bash
# Check consumer lag
docker exec kafka kafka-consumer-groups.sh --describe --group [GROUP] --bootstrap-server localhost:9092

# Reset consumer to latest
docker exec kafka kafka-consumer-groups.sh --reset-offsets --group [GROUP] --topic PetSitterPetEvents --to-latest --bootstrap-server localhost:9092 --execute
```

#### 7. Environment Variables Not Set
**Error**: `Environment variable KAFKA_TOPIC_PETSITTER_*_EVENTS is empty`

**Check**:
- Verify `.env` files or `launchSettings.json`
- Ensure environment variables are properly set:
  - `KAFKA_TOPIC_PETSITTER_PET_EVENTS=PetSitterPetEvents`
  - `KAFKA_TOPIC_PETSITTER_SITTER_EVENTS=PetSitterSitterEvents`

### Debug Workflow
1. **Check Container Status** → `docker ps`
2. **Check Topic Existence** → List and describe topics
3. **Monitor Event Flow** → Use console consumer
4. **Check Consumer Groups** → Verify lag and status
5. **Review Application Logs** → Command and Query side logs
6. **Test Event Publishing** → Trigger API calls and monitor
7. **Reset if Needed** → Consumer offsets or recreate topics

---

## 📚 Additional Resources

- [Kafka Documentation](https://kafka.apache.org/documentation/)
- [Confluent Kafka Docker Guide](https://docs.confluent.io/platform/current/quickstart/ce-docker-quickstart.html)
- [Bitnami Kafka Docker Hub](https://hub.docker.com/r/bitnami/kafka)

---

**Last Updated**: February 2026  
**Project**: CQRS Pet Sitter - Kafka Debugging Guide