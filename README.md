# Rapid Technologies Assessment - Employee Time Visualizer (C#)

A clean, modular C# application that fetches time entries from a remote Azure REST endpoint, aggregates and calculates employee work hours, and visualizes the results as:
1. **Interactive HTML Report (`employees.html`)**: A styled, responsive table ordered by total time worked, highlighting employees who worked less than 100 hours.
2. **Visual Pie Chart (`piechart.png`)**: A high-resolution chart showing the exact percentage breakdown of total time worked by each employee.

---

## 📊 Output Results Preview

### 1. Pie Chart Visualization (`piechart.png`)
![Employee Work Distribution Pie Chart](piechart.png)

### 2. HTML Table Report (`employees.html`)
The generated report features:
- **Executive Metric Cards**: Total hours worked, team count, average hours per employee, and alert counter.
- **Ordered Data Table**: Sorted descending by total hours worked.
- **Threshold Highlighting**: Table rows for employees with less than 100 hours worked are highlighted in soft alert red with dedicated status badges (`< 100 Hours`).
- **Embedded Visual Chart**: Seamless integration of the generated pie chart alongside the data table.

---

## 🏗️ Architecture & Project Structure

The project adheres to SOLID principles and clean architecture practices, using clear abstractions so each component has a single responsibility:

```
├── .github/
│   └── workflows/
│       └── ci.yml                     # Automated GitHub Actions CI workflow
├── src/
│   └── Rituraj_CSharp_Assignment/     # Main Application Project
│       ├── Models/
│       │   ├── TimeEntry.cs           # DTO mapping raw API schema (StarTimeUtc, DeletedOn, etc.)
│       │   └── EmployeeSummary.cs     # Aggregated employee statistics model
│       ├── Services/
│       │   ├── IApiService.cs         # Abstraction for REST API calls
│       │   ├── TimeEntryApiService.cs # HttpClient implementation fetching JSON
│       │   ├── IEmployeeReportService.cs  # Abstraction for data cleaning & aggregation
│       │   ├── EmployeeReportService.cs   # LINQ grouping, sorting, & data integrity handling
│       │   ├── IHtmlReportGenerator.cs    # Abstraction for HTML generation
│       │   ├── HtmlReportGenerator.cs     # Semantic HTML5 & responsive CSS table builder
│       │   ├── IPieChartGenerator.cs      # Abstraction for PNG chart rendering
│       │   ├── PieChartGenerator.cs       # SkiaSharp modern donut pie chart renderer
│       │   └── FontResolver.cs        # Cross-platform font resolver with embedded fallback
│       ├── Resources/
│       │   └── DejaVuSans.ttf         # Embedded font ensuring 100% OS-agnostic rendering
│       ├── Program.cs                 # Main entrypoint orchestrator
│       └── Rituraj_CSharp_Assignment.csproj
├── tests/
│   └── Rituraj_CSharp_Assignment.Tests/
│       ├── EmployeeReportServiceTests.cs  # Unit tests for business logic & thresholds
│       ├── HtmlReportGeneratorTests.cs    # Unit tests for HTML structure & row styling
│       └── Rituraj_CSharp_Assignment.Tests.csproj
├── employees.html                     # Generated output HTML report
├── piechart.png                       # Generated output PNG pie chart
├── Rituraj_CSharp_Assignment.sln      # Solution file
└── README.md
```

---

## 💡 Key Design Decisions & Business Rules

1. **REST API Data Ingestion**:
   - The Azure Function endpoint returns timestamps with the key `StarTimeUtc` (omitting the letter 't'). The `TimeEntry` model uses `[JsonPropertyName("StarTimeUtc")]` to cleanly map this property.
   - Deserialization is performed asynchronously with streaming via `System.Text.Json` for optimal memory and performance.

2. **Data Cleaning & Integrity**:
   - **Deleted Records**: Entries with `DeletedOn != null` are filtered out.
   - **Inverted Durations**: Records where `EndTimeUtc <= StartTimeUtc` are handled safely without producing negative hours.
   - **Realistic Name Handling**: Entries with missing or whitespace employee names are assigned a realistic fallback name (`Vijay (Unassigned)`) per the data integrity specifications, avoiding generic placeholders like `First_1`.

3. **Cross-Platform Graphics Rendering**:
   - Instead of legacy `System.Drawing.Common` (which is restricted to Windows in .NET 7/8+), the solution uses **SkiaSharp** with the `SkiaSharp.NativeAssets.Linux.NoDependencies` package.
   - An embedded open-source font (`DejaVuSans.ttf`) ensures crisp, consistent typography across Linux, macOS, Windows, and CI environments without requiring host OS font installations.

---

## 🚀 Quickstart Guide

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.

### 1. Run the Application
To fetch data from the API and generate `employees.html` and `piechart.png`:
```bash
dotnet run --project src/Rituraj_CSharp_Assignment/Rituraj_CSharp_Assignment.csproj
```

### 2. Run Unit Tests
To run the automated test suite:
```bash
dotnet test
```

### 3. Build Single-File Executable
To package the application into a single standalone executable:

**For Windows (x64):**
```bash
dotnet publish src/Rituraj_CSharp_Assignment/Rituraj_CSharp_Assignment.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true \
  -o publish/win
```
Produces `publish/win/Rituraj_C# Assignment.exe`.

**For Linux (x64):**
```bash
dotnet publish src/Rituraj_CSharp_Assignment/Rituraj_CSharp_Assignment.csproj \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o publish/linux
```
Produces `publish/linux/Rituraj_C# Assignment`.

---

## 🔄 Continuous Integration (CI)

A GitHub Actions workflow is preconfigured in `.github/workflows/ci.yml`. On every push and pull request to `main`:
1. Sets up the .NET 8 SDK environment.
2. Restores and builds the solution.
3. Executes all xUnit test suites.
4. Runs the visualizer application.
5. Builds standalone single-file executables for both Linux and Windows.
6. Publishes the output reports and executables as downloadable workflow artifacts.

---

## 📋 Summary Table

| Rank | Employee Name | Total Hours | Share (%) | Status |
|:---:|:---|:---:|:---:|:---:|
| 1 | Patrick Huthinson | 212.13 hrs | 13.08% | ✓ Compliant |
| 2 | Abhay Singh | 207.65 hrs | 12.80% | ✓ Compliant |
| 3 | Stewart Malachi | 197.83 hrs | 12.20% | ✓ Compliant |
| 4 | John Black | 197.02 hrs | 12.15% | ✓ Compliant |
| 5 | Mary Poppins | 167.92 hrs | 10.35% | ✓ Compliant |
| 6 | Tim Perkinson | 163.17 hrs | 10.06% | ✓ Compliant |
| 7 | Kavvay Verma | 158.12 hrs | 9.75% | ✓ Compliant |
| 8 | Rita Alley | 112.18 hrs | 6.92% | ✓ Compliant |
| 9 | **Raju Sunuwar** | 95.67 hrs | 5.90% | ⚠ **< 100 hrs** |
| 10 | **Tamoy Smith** | 86.78 hrs | 5.35% | ⚠ **< 100 hrs** |
| 11 | **Vijay (Unassigned)** | 23.50 hrs | 1.45% | ⚠ **< 100 hrs** |