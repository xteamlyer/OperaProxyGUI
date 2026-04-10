# Opera Proxy GUI

**WPF (.NET 8)** обёртка над `opera-proxy.windows-amd64.exe` с MVVM-архитектурой
и встроенным авто-подбором прокси для аргумента `-api-proxy`.

## Возможности

- **Нативный WPF + MVVM** — `ViewModelBase`, `RelayCommand`, привязки данных.
- **Тёмная тема** — собственная палитра в `Themes/Dark.xaml`, тёмный заголовок
  окна через DWM (`DarkTitleBar`).
- **Авто-подбор `-api-proxy`** — скачивает текстовый файл со списком прокси,
  проверяет каждое на успешное подключение и передаёт первый рабочий
  в аргумент `-api-proxy`.
- **Трей** — сворачивание в системный трей, контекстное меню, balloon-уведомление.
- **Автозапуск** — опция запуска прокси при старте программы и регистрация в
  реестре `HKCU\Run`.
- **Лог** — вывод stdout/stderr процесса с ограничением длины.

## Структура проекта

```
OperaProxyGUI/
├── App.xaml / App.xaml.cs          — точка входа, ресурсы приложения
├── Views/MainWindow.xaml / .cs     — главное окно (code-behind минимален)
├── Models/
│   ├── Enums.cs                    — CountryRegion, ServerSelectionPolicy
│   ├── Defaults.cs                 — значения по умолчанию
│   ├── AppSettings.cs              — конфиг (INotifyPropertyChanged) + JSON
│   ├── ProxyEntry.cs               — разбор строки прокси
│   └── ArgumentBuilder.cs          — сборка аргументов opera-proxy
├── Themes/
│   └── Dark.xaml                   — тёмная тема
├── ViewModels/
│   ├── ViewModelBase.cs            — INotifyPropertyChanged
│   ├── RelayCommand.cs             — ICommand
│   ├── MainViewModel.cs            — свойства и состояние
│   ├── MainViewModel.Commands.cs   — команды запуска/подбора прокси
│   └── MainViewModel.Helpers.cs    — лог, трей, автозапуск
├── Services/
│   ├── ConfigService.cs            — загрузка/сохранение JSON
│   ├── AutostartService.cs         — реестр HKCU\Run
│   ├── ProxyRunnerService.cs       — запуск/остановка процесса
│   ├── ProxyListService.cs         — скачивание и проверка прокси
│   ├── TrayIconService.cs          — иконка в трее
│   └── DarkTitleBar.cs             — тёмный заголовок через DWM
└── Resources/
    └── opera-proxy.windows-amd64   ← ПОЛОЖИТЕ БИНАРНИК СЮДА
```

## Авто-подбор прокси для `-api-proxy`

1. В блоке **«Авто-подбор прокси для -api-proxy»** включите чекбокс
   «Скачивать список прокси и выбирать рабочий».
2. Укажите **URL списка** — текстовый файл, одна строка — один прокси.
   Поддерживаются форматы:
   - `socks4://156.156.162.1:1080`
   - `socks5://156.156.162.1:1080`
   - `http://156.156.162.1:1080` / `https://156.156.162.1:1080`
   - `156.156.162.1:1080` (без схемы — трактуется как socks5)
3. При запуске прокси программа:
   - скачивает список,
   - парсит и отбрасывает невалидные строки,
   - параллельно (до `ProxyTestConcurrency` одновременно) проверяет каждое
     подключение с таймаутом `ProxyTestTimeoutMs`,
   - первый успешно подключившийся прокси передаётся как `-api-proxy`.
4. Кнопка **«Проверить»** позволяет протестировать список без запуска прокси.

Если ни один прокси не прошёл проверку — запуск отменяется, в лог пишется
соответствующее сообщение.

## Как встроить бинарник в exe

1. Создайте папку `Resources/` внутри проекта.
2. Скопируйте туда `opera-proxy.windows-amd64.exe`.
3. Соберите проект (`Release`). Итоговый `OperaProxyGUI.exe` содержит бинарник внутри.

При запуске программа распакует бинарник во `%TEMP%` и запустит оттуда.
При остановке — temp-файл удаляется.

## Fallback (без embed)

Если бинарник не встроен, программа ищет его в порядке:
1. `.\opera-proxy.windows-amd64.exe` (рядом с exe)
2. `.\bin\opera-proxy.windows-amd64.exe`
3. `%TEMP%\opera-proxy.windows-amd64.exe` (embedded)

## Сборка

```bash
dotnet build -c Release
# или
msbuild OperaProxyGUI.sln /p:Configuration=Release /p:Platform="Any CPU"
```

Требуется **.NET 8 SDK** (Windows-only, `net8.0-windows`).

## Поведение

| Действие | Результат |
|---|---|
| Закрыть окно (×) | Сворачивается в трей |
| Трей → Выход | Полное закрытие + остановка прокси |
| Кнопка «Остановить» | Убивает процесс и остаётся в трее |
| Двойной клик по трею | Показывает окно |

## Параметры

**Обязательные** (всегда передаются):
- `bind-address` — адрес:порт
- `socks-mode` — чекбокс
- `bootstrap-dns` — список DNS через запятую
- `country` — EU / AS / AM
- `verbosity` — число 0-100

**Необязательные** (передаются только если включён чекбокс слева):
- `api-user-agent`, `fake-SNI`, `init-retries`, `init-retry-interval`,
  `server-selection`, `server-selection-dl-limit`, `server-selection-timeout`,
  `server-selection-test-url`, `api-address`, `api-client-type`,
  `api-client-version`, `api-login`, `api-proxy`, `cafile`, `proxy`,
  `override-proxy-address`, `timeout`, `refresh`, `refresh-retry`
