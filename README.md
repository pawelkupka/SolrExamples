# Solr

## Document, Fields, Schema

- Document to podstawowa jednostka informacji, ktora zawiera dane opisujace cos (przypomina encje danych). 
- Document sklada sie z pol (fields). Np. document o ksiazce zawiera pola: tytul, autora, rok publikacji, liczba stron, opis, itd. 
- Kazde pole ma jakis typ (field type), np. text, float, itd.
- Pola moga zawierac dodatkowo analizy (field analysis), ktore pozwalaja prawidlowo indeksowac zlozone dane, np. opis ksiazki mozna podzielic na oddzielne slowa z malymi literami i ignorowac niepotrzbne slowa i znaki
- Wszystkie dane na temat dokumentow i ich pol sa trzymane w schemacie w pliku schema.xml (manualne zarzadzanie), albo managed-schema.xml (programowalne zarzadzanie)
- Przy zmianie schematu trzeba zawsze wlaczyc pelne reindeksowanie danych, inaczej solr moze nie dzialac prawidlowo

## Indexing

- Solr pozwala na dodawanie, aktualizowanie i usuwanie zindeksowanych danych
- Dane do Solra mozna wysylac za pomoca requestow HTTP w roznych formatach, np. JSON, XML
- Request do Solr powinien zawierac document z nazwami pol i wartosciami pol. Jedno z pol moze zawierac unikalny identyfikator UID
- Pola, ktore nie sa zdefiniowane w schema zostana zignorowane, albo zmapowane na [Dynamic Fields](https://solr.apache.org/guide/solr/latest/indexing-guide/dynamic-fields.html) (jesli takie zostaly zdefiniowane)

## Searching

- Kiedy uzytkownik uruchamia search query wowczas uruchomiony zostaje request handler, ktory jest pluginem
- Moga byc rozne rodzaje request handlerow, np. przetwarzajace zapytania, albo replikujace indeksy
- Aplikacja klienta moze sama wybierac ktorego handlera chce uzyc
- Przed uruchomieniem query uruchamiany jest query parser, ktory interpretuje parametry query
- Sa rozne rodzaje parserow, domyslny to [Standard Query Parser](https://solr.apache.org/guide/solr/latest/query-guide/standard-query-parser.html), inny to np. [DisMax Parser](https://solr.apache.org/guide/solr/latest/query-guide/dismax-query-parser.html) (bardziej odporny na bledy, podobny do google)
- Query moze zawierac: 
  - szukana fraze
  - parametry precyzyjnego zapytania (np. wykluczenie niektorych pol)
  - parametry do kontroli prezentacji (np. sortowanie, albo maksymalna liczba wynikow)
  - parametry filtrow (np. wyniki wyszukiwania moga byc dodatkowo filtrowane i zapisywane w cache). Uwaga! To nie to samo co Filter Analysis (tokenizer parsuje tresc zgodnie z podanymi regulami)
- Solr pozwala grupowac zwracane wyniki na dwa sposoby:
  - faceting - wyniki sa pogrupowane po kategoriach (zindeksowanych terminach)
  - clustering - wyniki sa pogrupowane wedlug podobienstw wykrytych podczas wyszukiwania (moga byc chaotyczne)
- Solr moze wykorzystac tez wyniki poprzedniego zapytania do kolejnego, bardziej precyzynjego wyszukiwania [MoreLikeThis](https://solr.apache.org/guide/solr/latest/query-guide/morelikethis.html)
- Koncowa odpowiedz z Solra jest generowana przez [Response Writer](https://solr.apache.org/guide/solr/latest/query-guide/response-writers.html)

## Clusters

- Node to pojedynczy serwer Solr
- Cluster to grupa node-ow Solr (serwerow)
- Istnieja dwa rodzaje zarzadzania klastrem:
  -  SolrCloud Mode - wykorzystuje ZooKeeper do zarzadzania klastrami. ZooKeeper sledzi stan kazdego Core-a w kazdym Node-ie. W tym trybie pliki konfiguracyjne znajduja sie w ZooKeeper, a nie w kazdym node-ie. Zadanie (zapytania lub indeksowania) moze przyjsc do dowolnego klastra, a ZooKeeper sam przekierowuje je do odpowiedniego node-a i core
  -  User-Managed Mode - zarzadzanie klastrami odbywa sie za pomoca lokalnych skryptow. Ten sposob wydaje sie kiepski, bo wszystko trzeba recznie konfigurowac i jest tego duzo
- Shard to fragment indeksu. Solr pozwala dzielic indeks na fragmenty i umieszczac je w osobnych node-ach (cos jak partycje). W trybie cloud indeks jest automatycznie dzielony jesli sa przynajmniej 2 shard-y, a w manual trzeba pisac skrypty
- Replica to kopia indeksu badz kopia shard-a, jesli jest skonfigurowany. Jest wykorzystywana podczas indeksowania, albo awarii glownej repliki (leader-a) lub przy duzej liczbie zapytan
- Leader to glowna replika, do ktorej wprowadza sie dane do indeksowania i z niej sa tworzone pozostale repliki (Followers)
- Core to poprostu replika (Leader i Followers). Node moze zawierac wiele Core-ow



## Exercise

1. Zrobic w API generator osob za pomoca biblioteki Bogus, ktory wygeneruje powiedzmy 10000 osob
2. Zrobic Cluster Solr Cloud z 3 node-ami i 3 shard-ami i kolekcja persons. Kazdy shard niech ma 3 Core-y typu NRT
3. Dodac schemar do kolekcji persons
4. Zindeksowac dane z API Persons w Solr
5. Wyprobowac rozne rodzaje queries
6. Dodac filtry

### 1. Zrobione za pomoca biblioteki Bogus. Endpoint url "http://localhost:5155/Persons/All"

### 2. Konfiguracja i uruchamianie Solr Cloud:
Uruchomienie Solr z przykladem z cloud. Komenda ```bin/solr.cmd -e cloud``` uruchomi interaktywny konfigurator. Wybrac 3 nody i ustawic trzy porty 8983, 8984, 8985.

To doda w katalogu solr/examples nowy katalog cloud z 3 nodami i uruchomi 3 nody na kazdym podanym porcie. Nastepnie wpisac nazwe kolekcji, moze byc persons. 

Nastepnie podzielic kolekcje na  3 shardy. I nastepnie podzielic kazdy shard na 3 core-y (repliki). I wpisac jako katalog dla config-ow "_default".

Po wszystkim Solr powinien dzialac pod adresem "http://localhost:8983/solr", "http://localhost:8984/solr" i "http://localhost:8985/solr".

Komenda ```solr.cmd status``` wyswietli statusy wszystkich node-ow.

Komenda ```solr.cmd stop -all``` zatrzyma wszystkie node-y.

Komenda ```solr.cmd stop -p 8983``` zatrzyma tylko node na porcie 8983.

Komenda ```solr.cmd restart -c -p 8983 -s ../example/cloud/node1/solr``` restartuje node na porcie 8983.

Komenda ```solr.cmd restart -c -p 8984 -z localhost:9983 -s ../example/cloud/node2/solr``` restartuje node na porcie 8984 wewnatrz klastra ZooKeepera ```localhost:9983```

Komenda ```solr.cmd start -cloud -s ../example/cloud/node3/solr -p 8985 -z localhost:9983``` dodaje node na porcie 8985 do klastra ZooKeepera ```localhost:9983```

Po wylaczeniu wszystkich nodow trzeba uruchomic pierwszy node bez ZooKeepera i pozostale podajac adres ZooKeepera:
- ```solr.cmd start -cloud -s ../example/cloud/node1/solr -p 8983```
- ```solr.cmd start -cloud -s ../example/cloud/node2/solr -p 8984 -z localhost:9983```
- ```solr.cmd start -cloud -s ../example/cloud/node3/solr -p 8985 -z localhost:9983```



Od poczatku

1. Uruchomic 3 node-y solr
```bin\solr.cmd start -c -p 8983```
```bin\solr.cmd start -c -p 8984 -z localhost:9983```
```bin\solr.cmd start -c -p 8985 -z localhost:9983```
Opcja "c" oznacza uruchomienie w trybie "cloud", opcja "p" oznacza numer portu node-a, opcja "z" oznacza adres servera ZooKeeper (domyslnie pierwszy node ma "localhost:9983").
W (panelu administracyjnym Solr) [http://localhost:8983/solr/#/~cloud] mozna zobaczyc zmiany. Trzeba odswiezyc ZooKepera jesli nie widac zmian.
Kolekcje powinny byc puste, jesli nie sa to skasowac przykladowe.
2. Tworzenie kolekcji
Mozna stworzyc kolekcje za pomoca api, albo w (panelu administracyjnym Solr) [http://localhost:8983/solr/#/~collections]. Nalezy podac nazwe, wybrac default config, ilosc shard-ow (np. 3) i ilosc replik w kazdym shard (np. 2). W "shard" mozna jeszcze podac nazwy shardow po przecinku (np. shardA,shardB,shardC)
3. Nalezy stworzyc schemat danych. Mozna recznie dodawac pola w kolekcji w menu schemas, albo za pomoca api. Uwaga! Niektore pola sa predefiniowane, np. "id", wiec trzeba uzyc innego
4. I teraz mozna juz importowac dane za pomoca api (wyslac zwykla tablice obiektow), albo dodawac recznie w kolekcji w menu documents. Jesli wysylane jest przez api to trzeba na koncu zrobic commit



5. Proby z roznymi query
Parametry:
-```defType```-okresla jakiego query parsera uzyc, domyslnie to Lucene
-```sort```-sortuje wyniki. Jest wiele sposobow na sortowanie, szczegoly (tutaj)[https://solr.apache.org/guide/solr/latest/query-guide/common-query-parameters.html]
-```start ```-oznacza pomijanie wierszy przy pobieraniu danych (cos jak Skip w LINQ)
-```rows ```-oznacza maksymalna liczbe wierszy przy pobieraniu danych (cos jak Take w LINQ)
-```canCancel```-oznacza czy query moze zostac anulowane w trakcie wykonywania
-```queryUUID```-unikalne query id potrzebne jesli chce sie anulowac query
-```fq ```-oznacza filter query, czyli query wynik skomplikowanego filtrowania jest cache-owany niezaleznie od glownego zapytania. Query moze zawierac wiele takich parametrow. Pozwala uzywac skladni (filter condition)[https://solr.apache.org/guide/solr/latest/query-guide/standard-query-parser.html#differences-between-lucenes-classic-query-parser-and-solrs-standard-query-parser]. Parametr cost pozwala ustawiac kolejnosc wykonywania filtrow query
-```cache ```-pozwala na wylaczenie cachowanie w filter query
-```fl```-pozwala wybrac ktore pola ma zwrocic query (cos jak Select w LINQ)
-```fl z funkcja```-pozwala robic obliczenia na zwracanych polach i wynik zwracac jako dodatkowe pseudopole (cos jak Sum, Avg w LINQ). (Lista dostêpnych funkcji)[https://solr.apache.org/guide/solr/latest/query-guide/function-queries.html]
-```fl z transformata```-pozwala transformowac zwracane wyniki, np. ustawiac formatowanie dokumentu
-```fl z aliasami```-aliasy pozwalaja zmieniac nazwy pol na inne (cos AS w SQL)
-```debug```-zwraca dodatkowe informacje o query, np. punktacja wyszukiwanych dokumentow, czas wyszukiwania, itp.
-```timeAllowed```-maksymalny czas wykonywania query, po tym czasie jest zwracane to co udalo sie znalezc do tego czasu
-```omitHeader```-pozwala wylaczyc dodatkowe informacje (responseHeader) zwracane w query takie czas wykonuwania itp.
-```wt```-okresla sposob formatowania wynikow odpowiedzi (domyslnie json)
-```logParamsList```-pozwala ograniczyc liste parametrow, ktore solr loguje przy kazdym query
-```echoParams```-pozwala kontrolowac informacje wyswietlane w responseHeader
-```minExactCount```-wyszukujac dokumenty solr nadaje im punktacje zalezna od liczby trafien. Parametr minExactCount okresla minimalna wartosc tej punktacji. Pozwala zwiekszych wydajnosc poprzez odrzucenie slabo pasujacych wynikow
Local Params:
-```q={!localparams}query```-pozwalaja zmienic zachowanie query, np. ustawic domyslne pole wyuszkiwania, albo zmienic domyslny operator z OR na AND, albo query parser, itp
JSON Request API:
Pozwala wysylac zapytania za pomoca json, a nie url. (Tutaj jest instrukcja)[https://solr.apache.org/guide/solr/latest/query-guide/json-request-api.html]
Searching Nested Child Documents:
(TODO)[https://solr.apache.org/guide/solr/latest/query-guide/searching-nested-documents.html]




6. Filter Cache

https://solr.apache.org/guide/solr/latest/configuration-guide/caches-warming.html#filter-cache




6. Zookeper

W dokumentacji (ZooKeeper Ensemble Configuration)[https://solr.apache.org/guide/solr/latest/deployment-guide/zookeeper-ensemble.html] rekomenduja, zeby uzywac wielu ZooKeeper-ow, ale nie wiecej niz 5.
W razie gdy jeden ZooKeeper przestanie dzialac to pozostale nadal moga dzialac zapewniajac dostep do pozostalych node-ow. Jest tam tez instrukcja jak skonfigurowac ZooKeepery