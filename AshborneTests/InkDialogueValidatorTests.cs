using AshborneTooling;

namespace AshborneTests
{
    public class InkDialogueValidatorTests : IDisposable
    {
        private enum FunctionType
        {
            SetFlag,
            SetCounter,
            SetLabel
        }

        private readonly string _testDirectory;

        public InkDialogueValidatorTests()
        {
            // Create a temporary directory for test files
            _testDirectory = Path.Combine(Path.GetTempPath(), $"InkValidatorTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            // Clean up test directory after tests
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
        }

        #region ValidateAllDialogues Tests

        [Fact]
        public void ValidateAllDialogues_NonExistentDirectory_ReturnsIssue()
        {
            // Arrange
            string nonExistentPath = Path.Combine(_testDirectory, "DoesNotExist");

            // Act
            var issues = InkDialogueValidator.ValidateAllDialogues(nonExistentPath);

            // Assert
            Assert.Single(issues);
            Assert.Contains("not found", issues[0].Message);
        }

        [Fact]
        public void ValidateAllDialogues_EmptyDirectory_ReturnsNoIssues()
        {
            // Arrange
            string emptyDir = Path.Combine(_testDirectory, "Empty");
            Directory.CreateDirectory(emptyDir);

            // Act
            var issues = InkDialogueValidator.ValidateAllDialogues(emptyDir);

            // Assert
            Assert.Empty(issues);
        }

        [Fact]
        public void ValidateAllDialogues_ValidDialogue_ReturnsNoIssues()
        {
            // Arrange
            string testFile = Path.Combine(_testDirectory, "valid.json");
            var validJson = CreateMockInkJson("TestFlag", "setFlag", FunctionType.SetFlag);
            File.WriteAllText(testFile, validJson);

            // Act
            var issues = InkDialogueValidator.ValidateAllDialogues(_testDirectory);

            // Assert
            Assert.Empty(issues);
        }

        [Fact]
        public void ValidateAllDialogues_MultipleFiles_AggregatesIssues()
        {
            // Arrange
            string file1 = Path.Combine(_testDirectory, "file1.json");
            string file2 = Path.Combine(_testDirectory, "file2.json");
            File.WriteAllText(file1, CreateMockInkJson("BadFlag1", "setFlag", FunctionType.SetFlag));
            File.WriteAllText(file2, CreateMockInkJson("BadFlag2", "setFlag", FunctionType.SetFlag));

            // Act
            var issues = InkDialogueValidator.ValidateAllDialogues(_testDirectory);

            // Assert
            Assert.True(issues.Count >= 2);
        }

        [Fact]
        public void ValidateAllDialogues_NestedDirectories_FindsAllFiles()
        {
            // Arrange
            string subDir = Path.Combine(_testDirectory, "SubFolder");
            Directory.CreateDirectory(subDir);
            string nestedFile = Path.Combine(subDir, "nested.json");
            File.WriteAllText(nestedFile, CreateMockInkJson("BadNestedFlag", "setFlag", FunctionType.SetFlag));

            // Act
            var issues = InkDialogueValidator.ValidateAllDialogues(_testDirectory);

            // Assert
            Assert.NotEmpty(issues);
            Assert.Contains(issues, i => i.FilePath == nestedFile);
        }

        #endregion

        #region ValidateSingleFile Tests

        [Fact]
        public void ValidateSingleFile_ReturnsNoIssues_WhenGivenValidJson()
        {
            // Arrange
            string testFile = Path.Combine(_testDirectory, "single_valid.json");
            var validJson = CreateMockInkJsonWithMultipleValidFunctions();
            File.WriteAllText(testFile, validJson);

            // Act
            var issues = InkDialogueValidator.ValidateSingleFile(testFile);

            // Assert
            Assert.Empty(issues);
        }

        [Fact]
        public void ValidateSingleFile_ReturnsNoIssues_WhenGivenValidJsonWithMultipleFunctions()
        {
            // Arrange
            string testFile = Path.Combine(_testDirectory, "single_valid_full.json");
            var validJson = CreateMockInkJsonWithMultipleValidFunctions();
            File.WriteAllText(testFile, validJson);

            // Act
            var issues = InkDialogueValidator.ValidateSingleFile(testFile);

            // Assert
            Assert.Empty(issues);
        }


        [Fact]
        public void ValidateSingleFile_ReturnsIssues_WhenGivenInvalidFunctionKeys()
        {
            // Arrange
            string testFile = Path.Combine(_testDirectory, "all_functions.json");
            var json = CreateMockInkJsonWithMultipleBadFunctions();
            File.WriteAllText(testFile, json);

            // Act
            var issues = InkDialogueValidator.ValidateSingleFile(testFile);

            // Assert
            Assert.NotEmpty(issues); // Should have issues for all the bad keys
            Assert.Contains(issues, i => i.Message.Contains("Flag"));
            Assert.Contains(issues, i => i.Message.Contains("Counter"));
            Assert.Contains(issues, i => i.Message.Contains("Label"));
        }
        
        #endregion

        #region PrintResults Tests

        [Fact]
        public void PrintResults_DoesntThrowException_WhenThereAreNoIssues()
        {
            // Arrange
            var issues = new List<InkDialogueValidator.ValidationIssue>();

            // Act & Assert
            InkDialogueValidator.PrintResults(issues);
            // Should complete without throwing
        }

        [Fact]
        public void PrintResults_ThrowsException_WhenThereAreIssues()
        {
            // Arrange
            var issues = new List<InkDialogueValidator.ValidationIssue>
            {
                new("test.json", "~ setFlag(BadKey)", "Test error")
            };

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                InkDialogueValidator.PrintResults(issues));
            
            Assert.Contains("validation failed", exception.Message);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a minimal mock Ink JSON file with a single external function call.
        /// This mimics the structure of real compiled Ink JSON.
        /// </summary>
        private string CreateMockInkJson(string keyName, string functionName, FunctionType functionType)
        {
            string secondParameter = functionType switch
            {
                FunctionType.SetFlag => "true",
                FunctionType.SetCounter => "1",
                FunctionType.SetLabel => "SomeLabelValue",
                _ => throw new ArgumentOutOfRangeException(nameof(functionType), functionType, null)
            };

            return $@"{{""inkVersion"":21,""root"":[[""ev"",""str"",""^{keyName}"",""/str"",{functionType},{{""x()"":""{functionName}"",""exArgs"":2}},""pop"",""/ev""]],""listDefs"":{{}}}}";
        }

        /// <summary>
        /// Creates a mock Ink JSON with multiple function types for comprehensive testing.
        /// </summary>
        private string CreateMockInkJsonWithMultipleBadFunctions()
        {
            return $@"{{""inkVersion"":21,""root"":[[""ev"",""str"",""^BadFlag"",""/str"",true,{{""x()"":""setFlag"",""exArgs"":2}},""pop"",""/ev"",""ev"",""str"",""^BadCounter"",""/str"",1,{{""x()"":""setCounter"",""exArgs"":2}},""pop"",""/ev"",""ev"",""str"",""^BadLabel"",""/str"",BadLabelValue,{{""x()"":""setFlag"",""exArgs"":2}},""pop"",""/ev""]],""listDefs"":{{}}}}";
        }

        /// <summary>
        /// Creates a mock Ink JSON with multiple valid function calls.
        /// </summary>
        private string CreateMockInkJsonWithMultipleValidFunctions()
        {
            return $@"{{""inkVersion"":21,""root"":[[""ev"",""str"",""^TestFlag"",""/str"",true,{{""x()"":""setFlag"",""exArgs"":2}},""pop"",""/ev"",""ev"",""str"",""^TestCounter"",""/str"",1,{{""x()"":""setCounter"",""exArgs"":2}},""pop"",""/ev"",""ev"",""str"",""^TestLabel"",""/str"",BadLabelValue,{{""x()"":""setLabel"",""exArgs"":2}},""pop"",""/ev""]],""listDefs"":{{}}}}";
        }

        #endregion
    }
}