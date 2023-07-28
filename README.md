# IAD

# 1. Prerequisite

To run the project, you will need:
IDE supporting both C# WPF and C++ projects. Preferably Rider or VisualStudio 2022.
Read and write privileges to the working directory.
That's about it really.

# 2. How to run

After opening the project in the IDE, you need to build all projects in the solution in all existing configurations

Existing configurations:
  #### 1. **Debug**
  #### 2. **DebugEditor**
  #### 3. **Release**
  #### 4. **ReleaseEditor**
  
When building for the first time or after code changes, the `Cannot find engine.lib` might appear. In that case, build the **Engine** first, then build the rest of the projects. After a succesful build, run the `IADEditor` project in either **DebugEditor** or **ReleaseEditor**.
