using ActionTimeRecorder.Models;
using ActionTimeRecorder.Services;
using Xunit;

namespace ActionTimeRecorder.Tests
{
    public class ActionServiceTests
    {
        private ActionService _sut;

        #region Constructor

        public ActionServiceTests()
        {
            _sut = new ActionService();
        }

        #endregion

        #region AddAction Tests

        [Theory(DisplayName = "AddAction Should Not Return Error for Valid Action")]
        [InlineData("{\"action\":\"jump\",\"time\":100}")]
        [InlineData("{\"action\":\"run\",\"time\":75}")]
        [InlineData("{\"action\":\"jump\",\"time\":200}")]
        public void AddAction_ShouldNotReturnErrorForValidAction(string action)
        {
            // Arrange
            var expectedResult = ActionMessages.Success;

            // Act
            var result = _sut.AddAction(action);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory(DisplayName = "AddAction Should Return Error for Invalid Action")]
        // Invalid action type
        [InlineData("{\"action\":\"fly\",\"time\":100}")]
        // Negative time
        [InlineData("{\"action\":\"jump\",\"time\":-1}")]
        // Missing property
        [InlineData("{\"time\":500}")]
        [InlineData("{\"action\":\"jump\"}")]
        // Missing value
        [InlineData("{\"action\":\"\",\"time\":100}")]
        [InlineData("{\"action\",\"time\":100}")]
        [InlineData("{\"action\":\"jump\",\"time\":}")]
        [InlineData("{\"action\":\"jump\",\"time\"}")]
        // Null value
        [InlineData("{\"action\":null,\"time\":100}")]
        [InlineData("{\"action\":\"jump\",\"time\":null}")]
        // Misspelled property
        [InlineData("{\"actoin\":\"jump\",\"time\":100}")]
        [InlineData("{\"action\":\"jump\",\"tmie\":100}")]
        // Missing all data
        [InlineData("{}")]
        public void AddAction_ShouldReturnErrorForInvalidAction(string action)
        {
            // Arrange
            var expectedResult = ActionMessages.ErrorInvalidAction(action);

            // Act
            var result = _sut.AddAction(action);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "Add Action Should Return Error for Empty Input")]
        public void AddAction_ShouldReturnErrorForEmptyInput()
        {
            // Arrange
            var expectedResult = ActionMessages.ErrorEmptyAction;

            // Act
            var result = _sut.AddAction(string.Empty);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region GetStats Tests

        #endregion
    }
}
