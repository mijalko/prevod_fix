# Subtitle Fixer

A tool for fixing and adjusting `.srt` and `.sub` subtitle files. Available as a **Windows desktop app** and a **web app** (Docker-ready).

## Features

- Shift all subtitles forward or backward by any number of seconds
- Remove entries that become negative after shifting
- Fix timing errors (end time before start time, zero-duration entries)
- Renumber entries sequentially starting from 1
- Preserves original encoding (UTF-8, UTF-16)

---

## Projects

| Project | Description |
|---|---|
| `Prevod` | Windows Forms desktop app |
| `PrevodWeb` | ASP.NET Core web app |
| `PrevodCore` | Shared subtitle processing library |

---

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker](https://www.docker.com/) (for the web app container)

### Run the Windows app

```bash
cd Prevod
dotnet run
```

### Run the web app locally

```bash
cd PrevodWeb
dotnet run
```

Then open `http://localhost:5388`.

### Run with Docker

```bash
docker compose up -d
```

Then open `http://localhost:5388`.

### Docker image

```
https://hub.docker.com/repository/docker/mijalko/subtitle-fixer/general
```

---

## Usage

1. Select or drag & drop a `.srt` or `.sub` file
2. Enter a time offset in seconds (e.g. `-10` to shift 10 seconds earlier, `+15` to shift later)
3. Click **Fix Subtitles**
4. Download the corrected file

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.
