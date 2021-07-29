# JumpCloudAssignment

Take-home interview assignment for JumpCloud. Provides two methods to add actions in the form of JSON strings and to get statistics on the average time of each action.

## Requirements

- Windows 10
- [Visual Studio 2019 Community Edition](https://visualstudio.microsoft.com/downloads/) or greater
- Command Prompt or PowerShell
- Git
- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1)

## Getting Started

To clone the repository, run the following in git:

```
git clone https://github.com/gmayo88/JumpCloudAssignment
```

To build and test the project you must be in the correct directory:

```
cd JumpCloudAssignment
```

To build the project run:

```
dotnet build
```

To run the unit tests run:

```
dotnet test
```

## Usage

The ActionService class provides two public methods: 

1) AddAction(string inputJson) : string
   - Accepts a JSON string representing an action and its time
     - Valid actions are "jump" and "run"
     - Example input:
     ```
     {"action":"run","time":100}
     ```
   - Example output: An error message if any problems occurred, or an empty string if the action was added successfully.

2) GetStats() : string
   - Returns a JSON string representing the average times for each action stored.
   - Averages are ordered by action type
     - Example output:
     ```
     [{"action":"jump","avg":100.0},{"action":"run","avg":125.0}]
     ```

## Example

Here is a simplified example program using both methods:

```
using ActionTimeRecorder;

void main()
{
  ActionService actionService = new ActionService();
  
  var actionOne = "{\"action\":\"run\",\"time\":500}";
  var actionTwo = "{\"action\":\"jump\",\"time\":100}";
  var actionThree = "{\"action\":\"run\",\"time\":50}";
  
  actionService.AddAction(actionOne);
  actionService.AddAction(actionTwo);
  actionService.AddAction(actionThree);
  
  var stats = actionService.GetStats();
  
  /*
    Output of the stats variable: 
    [
      {
        "action": "jump",
        "avg": 100.0
      },
      {
        "action": "run",
        "avg": 275.0
      }
    ]
  */
}

```

## Design

### Thread-safe Collections
This service stores actions in a ConconurrentDictionary using the ActionInfo's Action string as a key and an IEnumerable of ints as a value. When AddAction is called, the JSON input string is validated and, if successful, the Action and its time are added to the dictionary. If the key already exists, the method updates the value by calling the IEnumerable Append extension method, which creates a copy of the existing IEnumerable value with the newly added value at the end of the list. GetStats retrieves each IEnumerable in the ConcurrentDictionary and calculates the average value of each collection using Average().

ConcurrentDictionary was chosen for fast lookups and a smaller memory footprint.

An alternate implementation could use a ConcurrentBag. Each ActionInfo could be easily stored in the bag and retrieved by grouping the elements by the Action value. This would require that each time value be stored with its associated action string, causing a greater memory footprint. Alternatively, a separate bag holding only ints could be created for each action type, but this would make the code increasingly more messy with each new action type.

## Assumptions

1) Only the actions mentioned in the specification ("run" and "jump") are valid.
2) Time is non-negative.
3) Time will not exceed the max value of an int (2147483647).
4) Inputs and outputs are not case-sensitive.
5) If the action was added successfully, AddAction should return an empty string.
6) If no actions have been added, GetStats may return an empty array.
