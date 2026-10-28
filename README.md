# Procedural Planetary Texture Mapping Demo

A research project implementing a mathematical pipeline to map 2D procedural noise onto a 3D spherical surface using C#. No actual 3d renderer was used, instead the rendering was done by manipulating the pixels inside the bitmap buffer directly.

# Demo

https://github.com/user-attachments/assets/ee1ab794-b316-4393-be72-45d6029f17c3

## Technical Objective
The goal of this project was to implement a spherical UV mapping algorithm to transform flat, fractal noise into a wrapped texture suitable for a planetary sphere. After this there were plans to expand this into full planet visualization and generation pipeline, but only the intial renderer was created as a proof of concept after measuring performance.

## Technical Implementation

### 1. Toroidal Noise Sampling
To ensure seamless texture continuity, the initial noise generation is performed using a toroidal (torus-based) coordinate system. This approach prevents the visible artifacts and "seams" typically found at the edges of a standard 2D texture, providing a continuous, seamless noise field for the projection stage.

### 2. Spherical Projection and Rotation
The engine performs a spherical mapping to project the generated texture onto a 3D surface. By calculating the appropriate trigonometric transformations for each pixel, the 2D texture is distorted to simulate a sphere. Rotation is implemented by applying a continuous offset to the sampled coordinate range, creating the visual effect of a rotating celestial body.

### 3. Direct Buffer Manipulation and CPU Optimization
To minimize the rendering overhead inherent in high-level UI frameworks, the engine utilizes direct manipulation of the `WriteableBitmap` pixel buffer. By performing manual index calculations and direct byte array writes, the implementation bypasses expensive drawing primitives. The mathematical kernel is designed to be computationally portable, making it a candidate for migration to a GPU-based shader (HLSL) for high-performance, real-time rendering.

## Technical Constraints & Post-Mortem

This project was developed as a learning exercise to learn WPF (Windows Presentation Foundation) and coordinate geometry. 

**Architectural Limitation:**
Because the project was built within the WPF framework, the rendering is CPU-bound. While direct pixel manipulation is significantly faster than drawing primitive shapes, it still hits a performance ceiling when attempting to scale resolution or complexity in real-time. 

**Conclusion:**
The project successfully validates the mathematical logic of the spherical projection. However, for production-grade real-time graphics, the implementation would require a migration from the CPU-bound WPF pipeline to a GPU-accelerated API (such as DirectX, OpenGL, or a game engine like MonoGame) to handle the heavy computational load of high-resolution fragment shading.

## Tech Stack
* **Language:** C#
* **Framework:** .NET / WPF
* **Noise Library:** FastNoiseLite
