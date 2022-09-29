# Exercício 1 - Testes Unitários

# Enunciado

Crie um novo projeto do tipo ClassLibrary, utilizando o framework .Net de sua preferência, onde será construído um sistema de urna eletrônica.

O sistema deve realizar a eleição de prefeito de uma cidade. 

As seguintes classes e métodos devem ser criadas: 

## Classe Candidato: (Nome, Votos)

- Construtor
- Adicionar voto
- Retornar votos

## Classe Urna: (Vencedor, VotosVencedor, Candidatos, EleicaoAtiva)

- Construtor
- Resultado da eleição
- Cadastro de candidato
- Iniciar/Finalizar eleição
---
**Crie um novo projeto, na mesma Solution do projeto da urna eletrônica, do tipo xUnit Test. Nesse projeto, faça:**

Para a classe Candidato, crie primeiro os testes utilizando TDD. Construa os seguintes cenários de teste:

- Validar se a quantidade de votos está correta após o cadastro do candidato e após a inserção de um novo voto
- Validar se o nome do candidato está correto após o cadastro do candidato


Para a classe Urna, utilize o código dos métodos já prontos e construa os seguintes cenários de teste:

- Validar se o construtor está inserindo os dados iniciais corretamente
- Validar se a eleição está sendo iniciada/encerrada corretamente
- Validar se, ao cadastrar um candidato, o última candidato na lista é o mesmo que foi cadastrado
- Validar o método de votação quando é informado um candidato não cadastrado
- Validar o método de votação quando é informado um candidato cadastrado
- Validar o resultado da eleição
    
    ### Fonte classe Urna
    
    ```csharp
    public class Urna
    {
        public string? VencedorEleicao { get; set; }
        public int VotosVencedor { get; set; }
        public List<Candidato>? Candidatos { get; set; }
        public bool EleicaoAtiva { get; set; }
    
        public Urna()
        {
            VencedorEleicao = "";
            VotosVencedor = 0;
            Candidatos = new List<Candidato>();
            EleicaoAtiva = false;
        }
    
        public void IniciarEncerrarEleicao()
        {
            EleicaoAtiva = !EleicaoAtiva;
        }
    
        public bool CadastrarCandidato(string nome)
        {
            if (!string.IsNullOrEmpty(nome))
            {
                Candidato candidato = new Candidato(nome);
                Candidatos.Add(candidato);
                return true;
            }
    
            return false;
        }
    
        public bool Votar(string nome)
        {
            var candidatoEscolhido = Candidatos.Where(c => c.Nome.ToUpper() == nome.ToUpper()).FirstOrDefault();
    
            if (candidatoEscolhido is not null)
            {
                candidatoEscolhido.AdicionarVoto();
                return true;
            }
    
            return false;
        }
    
        public string MostrarResultadoEleicao()
        {
            var vencedor = Candidatos.OrderBy(c => c.RetornarVotos()).ThenBy(c => c.Nome).FirstOrDefault();
    
            return $"Nome vencedor: {vencedor.Nome}. Votos: {vencedor.Votos}";
        }
    }
    ```
    

Nos testes, utilize [Fact] ou [Theory] onde julgar necessário. Ao terminar, suba o fonte no seu próprio Github e envie o link no canal #temp-testes-unitarios-exerc01 no Discord.
