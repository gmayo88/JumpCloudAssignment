using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using ActionTimeRecorder.Enums;
using ActionTimeRecorder.Models;
using Newtonsoft.Json;

namespace ActionTimeRecorder.Services
{
    public class ActionService
    {
        private ConcurrentDictionary<string, IEnumerable<int>> _actionDictionary;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Error
        };

        public ActionService()
        {
            _actionDictionary = new ConcurrentDictionary<string, IEnumerable<int>>();
        }

        /// <summary>
        /// Reads the given input Json and adds
        /// </summary>
        /// <param name="inputJson"></param>
        /// <returns>Status of adding the action</returns>
        public string AddAction(string inputJson)
        {
            // Check for empty input string
            if (string.IsNullOrEmpty(inputJson))
            {
                return ActionMessages.ErrorEmptyAction;
            }

            // Make sure the input can be read into a valid ActionInfo
            var deserializeSuccess = TryDeserializeActionInfoJson(inputJson, out var actionInfo);

            if (!deserializeSuccess)
            {
                return ActionMessages.ErrorInvalidAction(inputJson);
            }

            // Add the action to the dictionary
            // Update values for any existing keys
            _actionDictionary.AddOrUpdate(actionInfo.Action, new[] {actionInfo.Time},
                (key, value) => value.Append(actionInfo.Time));

            return ActionMessages.Success;
        }

        /// <summary>
        /// Calculates the average time for each action type
        /// stored in _actionDictionary
        /// </summary>
        /// <returns>A Json string representing the array of averages</returns>
        public string GetStats()
        {
            // Convert each key/value pair in the
            // dictionary to an ActionStats object
            // containing the average time for the action
            var actionStats = _actionDictionary
                .OrderBy(x => x.Key)
                .Select(x => 
                {
                    return new ActionStats 
                    {
                        Action = x.Key,
                        Avg = x.Value.Average()
                    };
                });

            // Return the serialized object
            return JsonConvert.SerializeObject(actionStats);
        }

        /// <summary>
        /// Attempts to deserialize the input Json string
        /// into an ActionInfo object
        /// </summary>
        /// <param name="input">The Json string being deserialized</param>
        /// <param name="result">The resulting object from the deserialization</param>
        /// <returns>Success of the deserialization</returns>
        private bool TryDeserializeActionInfoJson(string input, out ActionInfo result)
        {
            result = null;
            try
            {
                var actionInfo = JsonConvert.DeserializeObject<ActionInfo>(input, _jsonSettings);

                // Make sure the action matches one of the valid types
                if (!Enum.TryParse(actionInfo.Action, true, out ActionType _))
                {
                    return false;
                }

                // Negative times are assumed to be invalid
                if (actionInfo.Time < 0)
                {
                    return false;
                }

                result = actionInfo;
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}