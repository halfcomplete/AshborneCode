using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Services;

namespace AshborneTests
{
    [Collection("AshborneTests")]
    public class SFXFormatterTests
    {
        [Fact]
        public void ConvertSFXToSpans_Succeeds_WithValidInput()
        {
            // Arrange
            var sfxString = "<sfx=sfx-shake><span style=\"color:#FFFFFF;\">\"Call me Ossaneth, the Unblinking Eye.\"</span></sfx=sfx-shake>";

            // Act
            var result = SFXFormatter.ConvertSFXToSpans(sfxString);

            // Assert
            Assert.Equal("<span class=\"sfx-shake\"><span style=\"color:#FFFFFF;\">\"Call me Ossaneth, the Unblinking Eye.\"</span></span>", result);
        }
    }
}
