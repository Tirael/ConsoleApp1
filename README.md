# Задание
Нужно написать консольное приложение поиска музыкальных альбомов исполнителя.

При вводе пользователем названия группы, программа должна запрашивать сервер в поисках списка её альбомов.

При отсутствии соединения с сервером, список (если он был загружен ранее) должен подгружаться из локального кэша.

Допускается использование любого сервера с любым API (рекомендуется сервис iTunes как не требующий авторизации).

Для хранения кэша допускается использование любого носителя (файл, любая база данных).
Допускается использование любых сторонних библиотек.

# Реализация
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