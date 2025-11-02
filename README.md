# MatchMaking POC - Local Setup

This guide will help you run the **MatchMaking service** with Kafka, Zookeeper, Redis, and workers locally using **Docker Compose**.

---

## Prerequisites

Make sure you have installed:

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)
- git

---

## 1. Setup the repository localy

```bash
git clone https://github.com/vasily-klubinkin/MatchMaking.git
cd MatchMaking
git checkout feature/initial
```

## 2. Run docker-compose

```bash
docker compose up --build -d --scale matchmaking-worker=2
```

### 3. Check Swagger

Open http://localhost:8080/swagger


### 4. Making requests

In Swagger, there are two endpoints:

- [PUT] /api/v1/matches/search – adds a user to the matchmaking search queue.
- [GET] /api/v1/matches – retrieves the most recently formed match.

Note: Kafka topics are created with 4 partitions, and the partitioning strategy distributes users across these partitions. To see a match actually formed, you need to add at least 12 users (4 partitions × 3 users per partition), as this represents the longest possible path for matchmaking.