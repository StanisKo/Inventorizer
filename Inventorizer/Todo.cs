/*
TODO:

1. Request in batches to speed things up

2. Implement pagination

3. !use IQuerable/IEnumerable in controllers = -- read from post tutorials! Since iterating over it is much faster than iterating over list
(
    this is also connected to pagination,
    since IQuerable puts the work on the database,
    while IEnumerable loads everything into memory

    Therefore, if you need to sort or filter the collection, or limit it, use IQueyrable
)

4. ForExService

5. Stats Service

6. Front End

7. Exception handling

8. Comments

Use proper names for lambdas

NOTE:

Stat service will have to translate USD to EUR since all prices are in USD
*/