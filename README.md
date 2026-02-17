# PlaywrightProject

Проект автоматизации тестирования с использованием Playwright и NUnit для C#.

## Структура проекта

- `VSProject/` - основной проект с тестами
- `VSProject/Tests/` - тестовые файлы (NUnit)
- `VSProject/Tests/Pages/` - Page Object Model классы
- `.github/workflows/` - конфигурация CI/CD

## Непрерывная интеграция (CI)

Проект настроен на автоматический запуск тестов через GitHub Actions.

### Когда запускаются тесты

- При push в ветки `main` и `Tests`
- При создании Pull Request в ветку `main`

### Что происходит в CI

1. Установка .NET 10.0 SDK
2. Восстановление зависимостей проекта
3. Сборка проекта в конфигурации Release
4. Установка браузеров Playwright (chromium)
5. Запуск всех тестов NUnit
6. Сохранение результатов тестирования как артефакты

### Просмотр результатов

Результаты тестов можно посмотреть в разделе **Actions** вашего репозитория на GitHub. Каждый запуск workflow создает артефакт `test-results` с файлами `.trx`, содержащими подробные результаты тестирования.

## Локальный запуск тестов

```bash
# Восстановление зависимостей
dotnet restore VSProject.slnx

# Сборка проекта
dotnet build VSProject.slnx --configuration Release

# Установка браузеров Playwright (только первый раз)
pwsh VSProject/bin/Release/net10.0/playwright.ps1 install --with-deps chromium

# Запуск тестов
dotnet test VSProject.slnx --configuration Release
```

## Настройка workflow

Файл конфигурации CI находится в `.github/workflows/dotnet-tests.yml`.

### Изменение версии .NET

Если нужно изменить версию .NET SDK, отредактируйте параметр `dotnet-version` в секции `Setup .NET`:

```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '10.0.x'  # Измените здесь версию
```

### Добавление других браузеров

Для тестирования в других браузерах (firefox, webkit), измените команду установки:

```yaml
- name: Ensure Playwright browsers are installed
  run: pwsh VSProject/bin/Release/net10.0/playwright.ps1 install --with-deps chromium firefox webkit
```
