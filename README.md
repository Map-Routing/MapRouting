# Map Routing

The project solves a real-world **shortest-path routing problem** on road maps, using **Dijkstra's Algorithm** with support for pedestrian walking radii around origin and destination points.

---

## Problem Description

Given a road map of intersections and roads, and a set of routing queries, the system finds the **optimal route** from a source location to a destination location.

Each query provides:

- The **(X, Y)** coordinates of the **source** and **destination** points (which may not sit exactly on an intersection).
- A **walking radius R** (in meters) — the maximum distance a person is willing to walk to/from a road intersection.

The system finds all intersections within radius **R** of the source and destination, then runs **Dijkstra's Algorithm** to compute the shortest time path between them.

---

## Algorithm & Core Logic

### Dijkstra's Algorithm (`shortestPath.cs`)

The core algorithm is an implementation of **Dijkstra's shortest path algorithm** using a **priority queue (min-heap)**.

- **Input:** A set of valid start nodes and end nodes (with pre-computed walking times as initial distances).
- **Edge weight:** Travel **time** in hours, calculated as `road.Length / road.Speed`.
- **Output:** The path (sequence of intersection IDs), total travel time, and total distances.

#### Steps:

1. All valid start-node intersections are enqueued with their walking time as the initial cost.
2. The algorithm relaxes neighbors iteratively, always processing the minimum-cost node first.
3. After completion, the best end node (minimizing `Dijkstra distance + walking time to destination`) is selected.
4. The path is reconstructed by following the parent array backwards from the best end node.

### Potential Nodes (`potentialNodes.cs`)

Before running Dijkstra, the system identifies **candidate intersections** within walking distance:

- For each intersection, the **Euclidean distance** to the source and destination coordinates is computed.
- Intersections within radius `R` (converted from meters to km) are added as valid start/end nodes.
- The walking time to each valid node is pre-computed assuming a walking speed of **5 km/h**.

---

## Data Model

| Class            | Description                                                                                                            |
| ---------------- | ---------------------------------------------------------------------------------------------------------------------- |
| `Map`          | Holds all `Intersection`s and adjacency lists of `Road`s (undirected graph)                                        |
| `Intersection` | A node with an integer `ID` and coordinates `(X, Y)` in km                                                         |
| `Road`         | A directed edge with `Source`, `Destination`, `Length` (km), `Speed` (km/h), and a computed `Time` (minutes) |
| `Query`        | A routing request: source coords, destination coords, and walking radius `R` (meters)                                |
| `Output`       | Per-query result: path (intersection IDs), shortest time (mins), total/walking/vehicle distances (km)                  |
| `TestCase`     | Groups a `Map` + `Queries` + `Outputs` for a single test run                                                     |

---

## Project Structure

```
MapRouting/
├── MapRoutingLogic/          # Core algorithms and data models
│   ├── Map.cs                # Graph structure (intersections + roads)
│   ├── Intersection.cs       # Node model
│   ├── Road.cs               # Edge model
│   ├── Query.cs              # Routing query model
│   ├── potentialNodes.cs     # Finds candidate intersections within radius R
│   ├── shortestPath.cs       # Dijkstra's algorithm + path reconstruction
│   ├── Output.cs             # Result model + equality comparison for testing
│   ├── TestCase.cs           # Test case container
│   ├── IO_Operations.cs      # File I/O, test case runner, parallel execution
│   ├── Program.cs            # Entry point
│   └── TEST CASES/           # Input/output test files
│       ├── [1] Sample Cases/
│       ├── [2] Medium Cases/
│       ├── [3] Large Cases/
│       └── [4] BONUS Test Cases/
└── MapRouting.sln
```

---

## Input Format

**Map file (`.txt`):**

```
<number_of_intersections>
<id> <x> <y>
...
<number_of_roads>
<source_id> <destination_id> <length_km> <speed_kmh>
...
```

**Query file (`.txt`):**

```
<number_of_queries>
<source_x> <source_y> <dest_x> <dest_y> <radius_meters>
...
```

---

## Output Format

For each query:

```
<intersection_id_1> <intersection_id_2> ... <intersection_id_n>
<shortest_time> mins
<total_distance> km
<walking_distance> km
<vehicle_distance> km
```

Followed by execution time (without I/O) and total execution time in milliseconds.

---

## 🚀 How to Run

1. Open `MapRouting.sln` in **Visual Studio** (or build with `dotnet build`).
2. Run the `MapRoutingLogic` project.
3. Choose a test case category at the prompt:
   - `[1]` Sample Cases (5 maps)
   - `[2]` Medium Cases (OpenLR)
   - `[3]` Large Cases (San Francisco)
   - `[4]` Custom — enter your own map and query file paths
4. Output files are saved under `TEST CASES/Output/`.

> **Note:** For queries ≥ 1,000, the system automatically switches to **parallel execution** (`Parallel.For`) to improve performance on large inputs.

---

## 🧪 Test Cases

Three tiers of test cases are provided:

| Tier   | Description                              |
| ------ | ---------------------------------------- |
| Sample | 5 small hand-crafted maps for validation |
| Medium | Real OpenLR map data                     |
| Large  | San Francisco road network               |

Expected outputs and case descriptions are in `TEST CASES/Test Cases Description & Expected Outputs.pdf`.

---

## 🛠️ Requirements

- **.NET 6.0+** (C#)
- Visual Studio 2022 or `dotnet` CLI
