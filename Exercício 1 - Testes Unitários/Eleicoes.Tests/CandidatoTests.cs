using FluentAssertions;

namespace Eleicoes.Tests
{
    public class CandidatoTests
    {
        #region Validar se a quantidade de votos est� correta ap�s o cadastro do candidato e ap�s a inser��o de um novo voto
        [Fact (DisplayName = "Cadastro de candidato deve iniciar com votos = 0")]
        [Trait("Urna Eletr�nica", "Candidato Testes")]
        public void Construtor_Inicializar_VotosIgualAZero()
        {
            // Arrange & Act
            var candidato = new Candidato("Jos� da Silva");
            var votos = candidato.Votos;

            // Act & Assert
            votos.Should().Be(0);
        }

        [Fact(DisplayName = "AdicionarVoto deve atualizar Votos")]
        [Trait("Urna Eletr�nica", "Candidato Testes")]
        public void AdicionarVoto_CandidatoValido_DeveAtualizarVotos()
        {
            // Arrange
            var candidato = new Candidato("Jos� da Silva");
            var votosEsperados = candidato.Votos + 1;

            // Act
            candidato.AdicionarVoto();

            // Assert
            Assert.Equal(votosEsperados, candidato.Votos);
        }
        #endregion

        #region Validar se o nome do candidato est� correto ap�s o cadastro do candidato
        [Fact(DisplayName = "Cadastro de candidato deve cadastrar nome corretamente")]
        [Trait("Urna Eletr�nica", "Candidato Testes")]
        public void Construtor_AdicionarNomeCandidato_DeveAdicionarCorretamente()
        {
            // Arrange
            var nomeCandidato = "Jos�";

            // Act
            var candidato = new Candidato(nomeCandidato);

            // Act & Assert
            candidato.Nome.Should().Be(nomeCandidato);
        }
        #endregion
    }
}