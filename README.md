#Агрегатор логов Apache
##Агрегатор логов Apache — это приложение, предназначенное для агрегации данных из access логов веб-сервера Apache и сохранения их в базу данных. Приложение поддерживает различные команды и фильтры для запросов и манипуляций с логами.

#Установка и запуск
Склонируйте репозиторий:

>git clone https://github.com/Antongo22/ApacheLogs.git
Перейдите в каталог проекта:
>cd ApacheLogs/bin/Debug


#Настройте конфигурационный файл config.txt

##Запустите приложение:
>ApacheLogs.exe

#Конфигурация
Ниже приведен пример конфигурации:

bin/Debug/config.txt:
>files_dir - путь к директории к логам
>ext - расширение логов
>format - формат логов
>time - автоматическое обновление логов в минутах
>showcron - true или false. Определяет, выводить ли отчёт об использованиие cron

При первом зауске программы, она сама создаст вам файл и откроет его в редакторе по умолчанию

#Виды запроса
>open - открывает в редакторе по умолчанию файл конфига.
>parse - получение данных конфига и логов
>close - завершает выполнение программы.
>getlog (date|datefrom) (dateto) (ip) (status) - получает данные логов из уже выгруженной базе данных.
>clear - очищает консоль и выводит доступные команды.

##Параметры
getlog:
без параметров: выводит все доступные логи
date|datefrom - если нетпараметра dateto, то поиск идёт только по этому дню, иначе будет считатся как стартовая дата для диапозона
dateto - по какой день делать выборку
ip - выводит только те логи, у которых есть даннй ip
status - выводит только те логи, у которых есть данный status

Примеры использования:

>getlog
>getlog 200
>getlog 192.168.2.20
>getlog 28.07.2006 25.08.2006 192.168.2.20
>getlog 28.07.2006 25.08.2006 192.168.2.20 200

Формат данных
Дата в запросе указывается в формате "dd-mm-yyyy", "dd.mm.yyyy", "dd-/mm/yyyy" или "dd-/mmm/yyyy". 
Примеры:
>28.07.2006
>25-08-2006
>09/Aug/2006

IP-адрес указывается в стандартном формате "ddd.ddd.ddd.ddd". Примеры:

192.168.2.20
127.0.0.1

#Выход из приложения
Для выхода из приложения используйте:
>close
