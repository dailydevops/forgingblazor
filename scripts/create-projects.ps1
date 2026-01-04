<#
.SYNOPSIS
  Creates multiple .NET projects and adds them to the solution.

.DESCRIPTION
  Automates creation of .NET projects for the ForgingBlazor solution. Uses a predefined list of
  project names, target directories, and project templates (e.g., classlib, xunit) to create
  each project via `dotnet new` and add it to the ForgingBlazor.slnx solution with proper solution
  folder mappings. The script looks for the solution file in the current directory and, if not
  found, in the parent directory before proceeding.

  The script performs the following operations:
  1. Locates the ForgingBlazor.slnx solution file in the current or parent directory
  2. Checks if each project already exists to prevent overwriting existing work
  3. Creates a project using the specified .NET template in the target directory
  4. Adds the created project to the solution with the correct solution folder structure

.NOTES
  File Name      : create-projects.ps1
  Prerequisite   : .NET SDK must be installed and available in PATH
  Solution File  : ForgingBlazor.slnx in current directory or parent directory

.EXAMPLE
  .\create-projects.ps1

  Creates all projects defined in the $projectNames array and adds them to the solution.
  Skips any projects that already exist.
#>

# Define project configurations as an array of tuples
# Each entry contains: (ProjectName, TargetDirectory, ProjectType)
# - ProjectName: The name of the .NET project to create
# - TargetDirectory: The folder where the project will be created (e.g., 'src' or 'tests')
# - ProjectType: The .NET project template to use (e.g., 'classlib', 'xunit', 'webapi')
$projectNames = @(
  @( "ForgingBlazor.Extensibility", "src", "classlib" ),
  @( "ForgingBlazor.Extensibility.Tests.Unit", "tests", "TUnit" ),
  @( "ForgingBlazor", "src", "classlib"),
  @( "ForgingBlazor.Tests.Integration", "tests", "TUnit" ),
  @( "ForgingBlazor.Tests.Unit", "tests", "TUnit" )
)
$solutionFile = ".\ForgingBlazor.slnx"

# Detect if the solution file exists, if not look into the parent directory
if (-not (Test-Path $solutionFile)) {
  $solutionFile = "..\ForgingBlazor.slnx"
  if (-not (Test-Path $solutionFile)) {
    Write-Error "Solution file ForgingBlazor.slnx not found in current or parent directory."
    exit 1
  }
}

# Process each project configuration
foreach ($project in $projectNames) {
  # Extract project name, target directory, and project type from the tuple
  $name = $project[0]
  $path = $project[1]
  $projectType = $project[2]

  # Verify if project already exists by checking for the .csproj file
  # This prevents accidental overwriting of existing projects
  if (Test-Path "$path\$name\$name.csproj") {
    Write-Host "Project $name already exists. Skipping creation." -ForegroundColor Cyan
    continue
  }

  # Create a new project using the .NET CLI with the specified template
  # $projectType: The template to use (e.g., 'classlib', 'xunit', 'webapi')
  # -n: Specifies the project name
  # -o: Specifies the output directory path
  dotnet new $projectType -n $name -o "$path\$name" | Out-Null
  Write-Host "Created project $name at $path\$name" -ForegroundColor Green

  # Add the newly created project to the solution file
  # -s: Specifies the solution folder where the project should appear in the solution explorer
  dotnet sln $solutionFile add "$path\$name\$name.csproj" -s $path | Out-Null
  Write-Host "Added project $name to solution under folder $path" -ForegroundColor Green
}

Write-Host "All projects processed." -ForegroundColor Cyan
