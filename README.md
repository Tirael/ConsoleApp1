[![Build status](https://img.shields.io/appveyor/ci/tirael/consoleapp1/master)](https://ci.appveyor.com/project/tirael/consoleapp1/branch/master)
[![CodeFactor](https://www.codefactor.io/repository/github/tirael/consoleapp1/badge/master)](https://www.codefactor.io/repository/github/tirael/consoleapp1/overview/master)

# Задание
Нужно написать консольное приложение поиска музыкальных альбомов исполнителя.

При вводе пользователем названия группы, программа должна запрашивать сервер в поисках списка её альбомов.

При отсутствии соединения с сервером, список (если он был загружен ранее) должен подгружаться из локального кэша.

Допускается использование любого сервера с любым API (рекомендуется сервис iTunes как не требующий авторизации).

Для хранения кэша допускается использование любого носителя (файл, любая база данных).
Допускается использование любых сторонних библиотек.

# Реализация консольного приложения
В качестве источника выбран *iTunes API*.

Для работы с *REST API* используется **RestSharp**.

Для кэширования используется **EasyCaching**. Для переключения кэша на диск/в sqlite в файле *appsettings.json* есть параметр *Properties:CacheToDb*.

Для логирования используется **Serilog**.

Для DI используется **Microsoft.Extensions.DependencyInjection**.


После запуска приложения необходимо ввести строку поиска - название исполнителя.

Первый запрос будет сохранен в кэш, последующие запросы будут имитировать ошибку при запросе (комментарии в *AppleMusicInfoProvider.cs*.

При повторе первой строки поиска - она будет взята из кэша.

Строка кэша валидна в течении одной минуты после добавления (константа **ExpirationCacheInMinutes** в *AppleMusicInfoProvider.cs*).

При перезапуске приложения кэш подгружается и доступен.

В файле *appsettings.json* настраивается путь к файловому кэшу и имя файла кэша для sqlite.

# Реализация WPF приложения
Добавлен проект *WpfApp1*.

Имитация ошибки при запросе удалена.

Запросы сохраняются в кэш sqlite.

Кэш изображений хранится в файловом кэше.

Для вывода звука используется **NAudio**, **Stateless**.

Для каркаса приложения WPF используется **Prism**, **ReactiveUI**.

Для темизации используется **MahApps**.
