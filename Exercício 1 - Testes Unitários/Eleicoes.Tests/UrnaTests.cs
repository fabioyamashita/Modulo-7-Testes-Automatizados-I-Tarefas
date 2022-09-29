using FluentAssertions;

namespace Eleicoes.Tests
{
    public class UrnaTests
    {
        #region Validar se o construtor está inserindo os dados iniciais corretamente
        [Fact(DisplayName = "Inicialização Urna")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        public void Construtor_Inicializar_DeveInicializarComDadosIniciais()
        {
            // Arrange
            string vencedorEleicao = "";
            int votosVencedor = 0;
            List<Candidato> candidatos = new List<Candidato>();
            bool eleicaoAtiva = false;

            // Act
            var urna = new Urna();

            // Assert
            urna.VencedorEleicao.Should().Be(vencedorEleicao);
            urna.VotosVencedor.Should().Be(votosVencedor);
            urna.Candidatos.Should().BeEquivalentTo(candidatos);
            urna.EleicaoAtiva.Should().Be(eleicaoAtiva);
        }
        #endregion

        #region Validar se a eleição está sendo iniciada/encerrada corretamente
        [Fact(DisplayName = "Encerrar Eleição")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        public void IniciarEncerrarEleicao_EleicaoAtiva_DeveAtualizarEleicaoAtivaParaFalse()
        {
            // Arrange
            var urna = new Urna();
            urna.EleicaoAtiva = true;

            // Act
            urna.IniciarEncerrarEleicao();

            // Assert
            urna.EleicaoAtiva.Should().BeFalse();
        }

        [Fact(DisplayName = "Iniciar Eleição")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        public void IniciarEncerrarEleicao_EleicaoNaoAtiva_DeveAtualizarEleicaoAtivaParaTrue()
        {
            // Arrange
            var urna = new Urna();
            urna.EleicaoAtiva = false;

            // Act
            urna.IniciarEncerrarEleicao();

            // Assert
            urna.EleicaoAtiva.Should().BeTrue();
        }
        #endregion

        #region Validar se, ao cadastrar um candidato, o última candidato na lista é o mesmo que foi cadastrado
        [Fact(DisplayName = "Cadastro de Candidato deve ser adicionado em última da lista")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        public void CadastrarCandidato_CandidatoValido_DeveSerUltimoDaListaDeCandidatos()
        {
            // Arrange
            var nomeCandidato1 = "José da Silva";
            var nomeCandidato2 = "Maria da Silva";

            var urna = new Urna();
            urna.CadastrarCandidato(nomeCandidato1);

            // Act
            urna.CadastrarCandidato(nomeCandidato2);

            // Assert
            urna.Candidatos?.Last().Nome.Should().Be(nomeCandidato2);
        }
        #endregion

        #region Validar o método de votação quando é informado um candidato não cadastrado
        [Theory(DisplayName = "Votar em candidato não cadastrado")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        [InlineData("Telma da Silva")]
        [InlineData("José de Alencar")]
        public void Votar_CandidatoNaoCadastrado_DeveRetornarFalse(string nomeCandidato)
        {
            // Arrange
            var nomeCandidato1 = "José da Silva";
            var nomeCandidato2 = "Maria da Silva";

            var urna = new Urna();
            urna.CadastrarCandidato(nomeCandidato1);
            urna.CadastrarCandidato(nomeCandidato2);

            // Act
            var result = urna.Votar(nomeCandidato);

            // Assert
            result.Should().BeFalse();
        }
        #endregion

        #region Validar o método de votação quando é informado um candidato cadastrado
        [Theory(DisplayName = "Votar em candidato cadastrado")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        [InlineData("José da Silva")]
        [InlineData("Maria da Silva")]
        public void Votar_CandidatoNaoCadastrado_DeveRetornarTrue(string nomeCandidato)
        {
            // Arrange
            var nomeCandidato1 = "José da Silva";
            var nomeCandidato2 = "Maria da Silva";

            var urna = new Urna();
            urna.CadastrarCandidato(nomeCandidato1);
            urna.CadastrarCandidato(nomeCandidato2);

            // Act
            var result = urna.Votar(nomeCandidato);

            // Assert
            result.Should().BeTrue();
        }
        #endregion

        #region Validar o resultado da eleição
        [Fact(DisplayName = "Validar o Resultado da Eleição")]
        [Trait("Urna Eletrônica", "Urna Testes")]
        public void MostrarResultadoEleicao_EleicaoValida_DeveRetornarMensagem()
        {
            // Arrange
            var nomeCandidato1 = "José da Silva";
            var nomeCandidato2 = "Maria da Silva";

            var urna = new Urna();
            urna.IniciarEncerrarEleicao();

            urna.CadastrarCandidato(nomeCandidato1);
            urna.CadastrarCandidato(nomeCandidato2);

            urna.Votar(nomeCandidato1);
            urna.Votar(nomeCandidato1);
            urna.Votar(nomeCandidato1);
            urna.Votar(nomeCandidato2);

            var expected = $"Nome vencedor: {nomeCandidato1}. Votos: 3";

            // Act
            var result = urna.MostrarResultadoEleicao();

            // Assert
            result.Should().Be(expected);
        }
        #endregion
    }
}
