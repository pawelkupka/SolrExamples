# Solr

## Document, Fields, Schema

- Document to podstawowa jednostka informacji, ktora zawiera dane opisujace cos. 
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