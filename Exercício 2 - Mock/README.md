# Descrição

Fazer projeto de testes para a PlayerService e testar o método GetForLeague.

## Solução

Foram feitos testes para os métodos GetByID e GetForLeague da classe PlayerService.

Foram feitos testes para o método Search da classe TeamService.

## Observações

Criei 2 tipos de testes (Ambos usando o Moq):
- Na pasta Services, criei os testes apenas usando uma classe. Algumas ferramentas utilizadas:
  - AutoFixture e Bogus para a criação de dados fake
  
- Na pasta Services_AutoMockerFixture, criei os testes usando algumas ferramentas como: 
  - AutoMocker 
  - ICollectionFixture
  - AutoFixture e Bogus para a criação de dados fake
