using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YeoruEXE
{
    [System.Serializable]
    public class PipeMessage
    {
        public string? command; // 명령어 (예: "change_color", "show_text")
        public string? value;   // 전달할 값 (예: "red", "Hello World")
                               // 나중에 필요하면 public int id; 같은 필드를 자유롭게 추가할 수 있습니다.
    }
}
