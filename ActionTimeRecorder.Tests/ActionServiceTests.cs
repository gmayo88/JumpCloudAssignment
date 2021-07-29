using ActionTimeRecorder.Models;
using ActionTimeRecorder.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        [Fact(DisplayName = "AddAction Should Return Error for Empty Input")]
        public void AddAction_ShouldReturnErrorForEmptyInput()
        {
            // Arrange
            var expectedResult = ActionMessages.ErrorEmptyAction;

            // Act
            var result = _sut.AddAction(string.Empty);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "AddAction Should Handle Async Requests")]
        public void AddAction_ShouldHandleAsyncRequests()
        {
            // Arrange
            var expectedResult = ActionMessages.Success;
            var results = new List<string>();

            // Act
            Parallel.For(0, 100, index => 
            {
                var actionInfo = new ActionInfo 
                {
                    Action = (index % 2 == 0) ? "jump" : "run",
                    Time = index
                };

                var inputJson = JsonConvert.SerializeObject(actionInfo);
                var result = _sut.AddAction(inputJson);
                results.Add(result);
            });

            // Assert
            Assert.All(results, result => Assert.Equal(expectedResult, result));
        }

        #endregion

        #region GetStats Tests

        [Fact(DisplayName = "GetStats Should Return Empty Array for No Actions")]
        public void GetStats_ShouldReturnEmptyArrayForNoActions()
        {
            // Arrange
            var expectedResult = "[]";

            // Act
            var result = _sut.GetStats();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact(DisplayName = "GetStats Should Return Correct Average for Single Action")]
        public void GetStats_ShouldReturnCorrectAverageForSingleAction()
        {
            // Arrange
            var expectedResult = "[{\"action\":\"run\",\"avg\":100.0}]";
            var action = "{\"action\":\"run\",\"time\":100}";

            // Act
            var addResult = _sut.AddAction(action);
            var getResult = _sut.GetStats();

            // Assert
            Assert.Equal(string.Empty, addResult);      // Add Action was successful
            Assert.Equal(expectedResult, getResult);
        }

        [Fact(DisplayName = "GetStats Should Return Correct Average for Multiple Actions")]
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

        [Fact(DisplayName = "AddAction and GetStats Should Handle Async Requests")]
        public async Task AddActionAndGetStats_ShouldHandleAsyncRequests()
        {
            // Arrange
            var timeToDelay = TimeSpan.FromMilliseconds(100);

            // Act
            var addTask = Task.Run(() =>
            {
                var addResults = string.Empty;
                for (int i = 0; i < 20; i++)
                {
                    var action = new ActionInfo()
                    {
                        Action = (i % 2 == 0) ? "jump" : "run",
                        Time = 10
                    };
                    var actionJson = JsonConvert.SerializeObject(action);
                    addResults += _sut.AddAction(actionJson);
                }
                return addResults;
            });

            var getTask = Task.Run(async () =>
            {
                var getResults = string.Empty;
                for (int i = 0; i < 10; i++)
                {
                    getResults += _sut.GetStats();
                    await Task.Delay(timeToDelay);
                }
                return getResults;
            });

            var results = await Task.WhenAll(addTask, getTask);

            // Assert
            Assert.True(string.IsNullOrEmpty(results[0]));
            Assert.False(string.IsNullOrEmpty(results[1]));
        }

        [Theory(DisplayName = "AddAction and GetStats Async Should Return Correct Results")]
        [InlineData(10, "[{\"action\":\"run\",\"avg\":4.5}]")]
        [InlineData(100, "[{\"action\":\"run\",\"avg\":49.5}]")]
        [InlineData(500, "[{\"action\":\"run\",\"avg\":249.5}]")]
        [InlineData(1000, "[{\"action\":\"run\",\"avg\":499.5}]")]
        [InlineData(10000, "[{\"action\":\"run\",\"avg\":4999.5}]")]
        public void AddActionAndGetStats_AsyncShouldReturnCorrectResults(int iterations, string expectedResult)
        {
            // Arrange
            var addActionResponses = new List<string>();

            // Act
            Parallel.For(0, iterations, index =>
            {
                var actionInfo = new ActionInfo
                {
                    Action = "run",
                    Time = index
                };

                var actionJson = JsonConvert.SerializeObject(actionInfo);
                var response = _sut.AddAction(actionJson);
            });

            var getStatsResult = _sut.GetStats();

            // Assert

            // AddAction should return successfully each time it is called
            Assert.All(addActionResponses, result => Assert.True(string.IsNullOrEmpty(result)));
            Assert.Equal(expectedResult, getStatsResult);
        }

        #endregion
    }
}
