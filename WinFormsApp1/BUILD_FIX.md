# Build fix — Windows long path / OneDrive error

## What the error means

Visual Studio cannot copy NuGet DLLs into:

`...\OneDrive - Illinois Tool Works, Inc\Gaming Laptop backup\ERP-...\WinFormsApp1\WinFormsApp1\bin\Debug\net10.0-windows\...`

That full path is **longer than Windows’ ~260 character limit**, so you get:

> Could not find a part of the path  
> Exceeded retry count of 10

This is **not** an application code bug.

## Fix (recommended)

1. Close Visual Studio.
2. Move / clone the repo to a **short local path** (not OneDrive), e.g.:

```text
C:\Dev\ERP
```

```powershell
git clone https://github.com/Rajkumarkanthasamy/ERP.git C:\Dev\ERP
cd C:\Dev\ERP
git checkout cursor/fix-long-path-build-9db7
```

3. Open `WinFormsApp1\WinFormsApp1.slnx`
4. In Visual Studio: **Build → Clean Solution**, then **Rebuild**

## Fix already in this branch

- Removed unused `Microsoft.Data.SqlClient` (app uses `System.Data.SqlClient` only) — drops deep `runtimes\win-x64\native\...SNI.dll` copies
- Shortened output folder from `bin\Debug\net10.0-windows\` to `bin\Debug\`

## If it still fails

1. Delete these folders manually, then rebuild:
   - `WinFormsApp1\WinFormsApp1\bin`
   - `WinFormsApp1\WinFormsApp1\obj`
2. Enable Windows long paths (Admin PowerShell):

```powershell
New-ItemProperty -Path "HKLM:\SYSTEM\CurrentControlSet\Control\FileSystem" `
  -Name "LongPathsEnabled" -Value 1 -PropertyType DWORD -Force
```

Then reboot.

3. Pause OneDrive sync for the project folder, or keep the project outside OneDrive entirely.
