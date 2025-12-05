This project implements a machine learning model (MLM) that learns how to build stable bridges using trial-and-error and evolutionary reinforcement learning.
It does not use a genetic algorithm. Instead, it uses Population-Based Training (PBT) combined with a neural network policy to gradually improve its bridge-building ability.

**How the Project Works**

Each model uses a small neural network to decide where to place bridge bars.
The network has three layers:

- Input layer (20 values) → environment observations

- Hidden layer (48 neurons, ReLU activation) → learns structural patterns

- Output layer (5 parameters × number of bars) → bar start, end, angle, length, type

The network then outputs a sequence of bar placements that form a complete bridge.



The neural network receives numerical information describing the world, such as:

- Anchor positions

- Width and height of the span

- Goal position

- Last node position

- Car progress across level

- Bar failures

- Safety and terrain information



For each bar in the bridge:

The network predicts start position, end position, angle, length, and type.

Noise is added for exploration (so it tries new ideas, this should allow it to not get stuck in a rut as eventually it will come across a random chance that gets more points).

The BarCreator then attempts to place each bar,

If a placement is invalid (too long, inside terrain, etc.), it fails and is skipped, resulting in lower scores which allow the model to grow out of this

After the model places its bars, a car attempts to drive across the bridge, then a fitness score is computed on a scale clamped up to a max of 100 (Unless I left it clamped from -999 to 999)

Each generation runs 20 models, each model gets assigned a fitness, the elites with the 2 highest fitnesses get to reproduce in the next generation

