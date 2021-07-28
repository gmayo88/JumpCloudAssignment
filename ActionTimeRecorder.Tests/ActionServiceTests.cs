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

        [Fact(DisplayName = "Get Stats Should Return Empty Array for No Actions")]
        public void GetStats_ShouldReturnEmptyArrayForNoActions()
        {
            // Arrange
            var expectedResult = "[]";

            // Act
            var result = _sut.GetStats();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "Get Stats Should Return Correct Average for Single Action")]
        public void GetStats_ShouldReturnCorrectAverageForSingleAction()
        {
            // Arrange
            var expectedResult = "[{\"action\":\"run\",\"avg\":100}]";
            var action = "{\"action\":\"run\",\"time\":100}";

            // Act
            var addResult = _sut.AddAction(action);
            var getResult = _sut.GetStats();

            // Assert
            Assert.Equal(string.Empty, addResult);      // Add Action was successful
            Assert.Equal(expectedResult, getResult);
        }

        [Fact(DisplayName = "Get Stats Should Return Correct Average for Multiple Actions")]
        public void GetStats_ShouldReturnCorrectAverageForMultipleActions()
        {
            // Arrange
            var expectedResult = "[{\"action\":\"jump\",\"avg\":75.0},{\"action\":\"run\",\"avg\":200.0}]";

            // Act
            var addResult1 = _sut.AddAction("{\"action\":\"run\",\"time\":100}");
            var addResult2 = _sut.AddAction("{\"action\":\"jump\",\"time\":25}");
            var addResult3 = _sut.AddAction("{\"action\":\"run\",\"time\":300}");
            var addResult4 = _sut.AddAction("{\"action\":\"jump\",\"time\":125}");

            var getResult = _sut.GetStats();

            // Assert
            Assert.Equal(string.Empty, addResult1);
            Assert.Equal(string.Empty, addResult2);
            Assert.Equal(string.Empty, addResult3);
            Assert.Equal(string.Empty, addResult4);     // Each Add Action was successful

            Assert.Equal(expectedResult, getResult);
        }

        #endregion
    }
}
