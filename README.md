# WypozyczalniaSamochodow

<p align="center">
  <a href="https://dotnet.microsoft.com/">
    <img src="https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white&style=for-the-badge" alt=".NET">
  </a>
  <a href="https://github.com/janpuc/WypozyczalniaSamochodow">
    <img src="https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white&style=for-the-badge" alt="C#">
  </a>
  <a href="https://github.com/janpuc/WypozyczalniaSamochodow/issues">
    <img src="https://img.shields.io/github/issues/janpuc/WypozyczalniaSamochodow?style=for-the-badge" alt="Issues">
  </a>
  <a href="https://github.com/janpuc/WypozyczalniaSamochodow/commits">
    <img src="https://img.shields.io/github/last-commit/janpuc/WypozyczalniaSamochodow?style=for-the-badge" alt="Last Commit">
  </a>
</p>

<p align="center">
  <b>Modułowy system do zarządzania wypożyczalnią samochodów</b> zbudowany w C#/.NET z wykorzystaniem architektury warstwowej i podejścia TDD.
</p>

---

## Spis treści

- [Opis projektu](#opis-projektu)
- [Funkcjonalności](#funkcjonalności)
- [Architektura](#architektura)
- [Technologie](#technologie)
- [Wymagania wstępne](#wymagania-wstępne)
- [Uruchomienie](#uruchomienie)
- [Testy](#testy)
- [Autorzy](#autorzy)
- [Konwencje projektowe](#konwencje-projektowe)

---

## Opis projektu

**WypozyczalniaSamochodow** to aplikacja demonstrująca zastosowanie nowoczesnych praktyk inżynierii oprogramowania w ekosystemie .NET. Projekt realizuje domenę wypożyczalni pojazdów, obejmującą zarządzanie flotą, procesy rezerwacji oraz logikę biznesową związaną z kalkulacją kosztów wynajmu.

> [!TIP]
> Aplikacja została zaprojektowana z podziałem na niezależne warstwy architektoniczne, co ułatwia testowanie, rozwój i wymianę komponentów infrastruktury.

---

## Funkcjonalności

- **Zarządzanie pojazdami** – dodawanie, edycja i śledzenie dostępności samochodów w flocie
- **Proces wypożyczania** – obsługa cyklu życia rezerwacji od utworzenia po zwrot pojazdu
- **Walidacja danych** – silna walidacja danych wejściowych na poziomie warstwy domeny
- **Kalkulacja kosztów** – logika biznesowa uwzględniająca okres wypożyczenia, kaucję i dodatkowe parametry
- **Wysokie pokrycie testowe** – wydzielony projekt testowy z testami jednostkowymi dla każdej warstwy aplikacji

---

## Architektura

Projekt oparty jest na architekturze **warstwowej** (Layered Architecture), co zapewnia separację odpowiedzialności oraz izolację logiki biznesowej od szczegółów technicznych.

```
┌─────────────────────────────────────┐
│         Presentation                │
│   (UI / API / Kontrolery)           │
├─────────────────────────────────────┤
│         Application                 │
│   (Serwisy, przypadki użycia, DTO)  │
├─────────────────────────────────────┤
│           Domain                    │
│   (Encje, reguły biznesowe)         │
├─────────────────────────────────────┤
│        Infrastructure               │
│   (Repozytoria, baza danych)        │
└─────────────────────────────────────┘
```

### Szczegółowy podział warstw

| Warstwa | Odpowiedzialność | Projekt testowy |
|:---|:---|:---|
| **Presentation** | Interfejs użytkownika, kontrolery, widoki | `Presentation/` |
| **Application** | Przypadki użycia, serwisy aplikacyjne, DTO | `Application/` |
| **Domain** | Encje, reguły biznesowe, obiekty wartości | `Domain/` |
| **Infrastructure** | Repozytoria, dostęp do bazy danych, zewnętrzne usługi | `Infrastructure/` |



---

## Technologie

- **C#** – główny język programowania
- **.NET** – platforma uruchomieniowa
- **Entity Framework Core** – mapowanie obiektowo-relacyjne (ORM)
- **ASP.NET Core** – warstwa prezentacji i API
- **xUnit / NUnit / MSTest** – frameworki testowe dla ekosystemu .NET
- **Moq / NSubstitute** – biblioteki do mockowania zależności w testach
- **Git + GitHub** – system kontroli wersji i platforma współpracy

---

## Wymagania wstępne

Przed uruchomieniem upewnij się, że masz zainstalowane:

- [.NET SDK](https://dotnet.microsoft.com/download) (zalecana wersja wskazana w pliku `global.json` lub `.csproj`)
- **Git** – do sklonowania repozytorium
- Jedno z wybranych IDE:
  - [Visual Studio](https://visualstudio.microsoft.com/)
  - [VS Code](https://code.visualstudio.com/) z rozszerzeniem C# Dev Kit
  - [NeoVim](https://neovim.io/) z konfiguracją LSP dla C#

---

## Uruchomienie

### 1. Klonowanie repozytorium

```bash
git clone https://github.com/janpuc/WypozyczalniaSamochodow.git
cd WypozyczalniaSamochodow
```

### 2. Przywrócenie zależności i kompilacja

```bash
# Przywrócenie pakietów NuGet
dotnet restore

# Kompilacja rozwiązania
dotnet build
```

### 3. Uruchomienie aplikacji

```bash
dotnet run --project WypozyczalniaSamochodow
```

> [!NOTE]
> W zależności od konfiguracji środowiskowej, przed pierwszym uruchomieniem może być konieczne skonfigurowanie connection stringa lub wykonanie migracji bazy danych.

---

## Testy

Projekt zawiera rozbudowany zestaw testów jednostkowych w katalogu `WypozyczalniaSamochodow.Tests`.

### Uruchomienie wszystkich testów

```bash
dotnet test
```

### Uruchomienie testów dla konkretnej warstwy

```bash
# Przykład: testy warstwy domeny
dotnet test --filter "FullyQualifiedName~Domain"
```

### Struktura katalogów testowych

```text
WypozyczalniaSamochodow.Tests
├── Domain/
├── Application/
├── Infrastructure/
├── Presentation/
├── TestSupport/
└── TestAssemblyConfig.cs
```

> [!TIP]
> Testy pisane są zgodnie z podejściem **TDD (Test-Driven Development)** – każda istotna funkcjonalność posiada odpowiadający jej zestaw przypadków testowych, co zapewnia wysoką odporność na regresje.

---

## Autorzy

| Osoba | Wkład |
|:---|:---|
| **Jan Puciłowski** | Architektura rozwiązania, konfiguracja repozytorium, backend |
| **Darren Stasiak** | Logika aplikacyjna, warstwa domenowa |
| **Kacper Oprządek** | Infrastruktura, warstwa testowa, wsparcie techniczne |

---

## Konwencje projektowe

W projekcie przyjęto szereg ustandaryzowanych konwencji, które zapewniają spójność kodu wieloosobowego zespołu pracującego na różnych edytorach:

- **Gitmoji** – dzięki plikowi `.gitmojirc.json` wiadomości commitów wykorzystują standaryzowane emoji (np. `:sparkles:`, `:white_check_mark:`), co znacząco poprawia czytelność historii zmian.
- **`.editorconfig`** – wymusza jednolite formatowanie (wcięcia, końce linii, kodowanie) niezależnie od użytego IDE (Visual Studio, VS Code, NeoVim).
- **XML Documentation Comments** – publiczne API opatrzone dokumentacją generującą podpowiedzi IntelliSense.
- **PascalCase / camelCase** – zgodne z oficjalną konwencją Microsoft dla języka C#.

---

## Licencja

Projekt powstał w ramach prac zespołowych. Szczegóły licencji określone są w pliku `LICENSE` (jeśli obecny w repozytorium).
