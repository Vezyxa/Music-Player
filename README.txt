                                        Документация проекта Музыкальный плеер
Описание проекта:
Музыкальный плеер представляет собой настольное приложение для управления аудиотреками и плейлистами. Пользователи могут добавлять, воспроизводить, удалять треки, а также создавать и управлять плейлистами.

Архитектура приложения:
Приложение построено с использованием модели MVVM (Model-View-ViewModel), что обеспечивает четкое разделение данных приложения, его представления и логики. Основные компоненты включают:
Model: представляет данные и бизнес-логику.
View: отвечает за визуальное представление данных.
ViewModel: связывает модель и вид, обеспечивая независимость.

Используемые технологии и библиотеки:
C# и .NET Framework: основные языки программирования для разработки приложения.
WPF (Windows Presentation Foundation): используется для создания графического интерфейса пользователя.
NAudio: библиотека для работы с аудио.
Newtonsoft.Json: библиотека для работы с JSON.
TagLib#: библиотека для обработки метаданных аудиофайлов.

                                              Инструкции по установке

Клонирование репозитория:
git clone <URL репозитория>

Открытие проекта:
Откройте решарперский или Visual Studio.
Перейдите в папку проекта и откройте файл проекта .csproj.

Сборка и запуск:
Сборка проекта с помощью Visual Studio.
Запуск отладочной версии для тестирования функциональности.

Основные функции приложения:
Добавление треков: возможность добавлять аудиофайлы в плейлисты.
Управление плейлистами: создание, удаление, просмотр треков в плейлистах.
Воспроизведение аудио: простое воспроизведение, пауза и остановка треков.
Регулировка громкости и положения воспроизведения.

                                                 Примеры использования

Добавление трека:
Нажмите кнопку Add Track.
Выберите файлы аудио.

Создание плейлиста:
Введите название в поле для названия нового плейлиста.
Нажмите кнопку New Playlist.

Удаление трека из плейлиста:
Выберите трек.
Нажмите кнопку Delete.

Этот музыкальный плеер обеспечивает пользователей инструментами для простой и удобной работы с музыкальными файлами и плейлистами в Windows.