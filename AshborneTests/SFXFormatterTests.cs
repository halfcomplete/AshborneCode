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
        public void ConvertSFXToSpans_Succeeds_WithNestedSpans()
        {
            // Arrange
            var sfxString = "<sfx=sfx-shake><span style=\"color:#FFFFFF;\">\"Call me Ossaneth, the Unblinking Eye.\"</span></sfx=sfx-shake>";

            // Act
            var result = SFXFormatter.ConvertSFXToSpans(sfxString);

            // Assert
            Assert.Equal("<span class=\"sfx-shake\"><span style=\"color:#FFFFFF;\"><span class=\"sfx-char\" style=\"--i:0\">\"</span><span class=\"sfx-char\" style=\"--i:1\">C</span><span class=\"sfx-char\" style=\"--i:2\">a</span><span class=\"sfx-char\" style=\"--i:3\">l</span><span class=\"sfx-char\" style=\"--i:4\">l</span> <span class=\"sfx-char\" style=\"--i:5\">m</span><span class=\"sfx-char\" style=\"--i:6\">e</span> <span class=\"sfx-char\" style=\"--i:7\">O</span><span class=\"sfx-char\" style=\"--i:8\">s</span><span class=\"sfx-char\" style=\"--i:9\">s</span><span class=\"sfx-char\" style=\"--i:10\">a</span><span class=\"sfx-char\" style=\"--i:11\">n</span><span class=\"sfx-char\" style=\"--i:12\">e</span><span class=\"sfx-char\" style=\"--i:13\">t</span><span class=\"sfx-char\" style=\"--i:14\">h</span><span class=\"sfx-char\" style=\"--i:15\">,</span> <span class=\"sfx-char\" style=\"--i:16\">t</span><span class=\"sfx-char\" style=\"--i:17\">h</span><span class=\"sfx-char\" style=\"--i:18\">e</span> <span class=\"sfx-char\" style=\"--i:19\">U</span><span class=\"sfx-char\" style=\"--i:20\">n</span><span class=\"sfx-char\" style=\"--i:21\">b</span><span class=\"sfx-char\" style=\"--i:22\">l</span><span class=\"sfx-char\" style=\"--i:23\">i</span><span class=\"sfx-char\" style=\"--i:24\">n</span><span class=\"sfx-char\" style=\"--i:25\">k</span><span class=\"sfx-char\" style=\"--i:26\">i</span><span class=\"sfx-char\" style=\"--i:27\">n</span><span class=\"sfx-char\" style=\"--i:28\">g</span> <span class=\"sfx-char\" style=\"--i:29\">E</span><span class=\"sfx-char\" style=\"--i:30\">y</span><span class=\"sfx-char\" style=\"--i:31\">e</span><span class=\"sfx-char\" style=\"--i:32\">.</span><span class=\"sfx-char\" style=\"--i:33\">\"</span></span></span>", result);
        }
        [Fact]
        public void ConvertSFXToSpans_DoesNothing_WhenNoSFXTags()
        {
            // Arrange
            var sfxString = "<span style=\"color:#FFFFFF;\">\"Call me Ossaneth, the Unblinking Eye.\"</span>";

            // Act & Assert
            Assert.Equal(sfxString, SFXFormatter.ConvertSFXToSpans(sfxString));
        }
    }
}
