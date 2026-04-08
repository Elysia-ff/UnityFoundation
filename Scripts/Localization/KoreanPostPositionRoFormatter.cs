using JetBrains.Annotations;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

namespace Elysia.Localizations
{
    /// <summary>
    /// <see cref="KoreanPostPositionFormatter"/> 의 예외에 대응하기 위한 Formatter.<br/>
    /// ex) 부사격조사 [(으)로]는 받침이 없거나 ㄹ 인 경우 [로], 그 외는 [으로].<br/>
    /// <br/>
    /// usage:<br/>
    /// {[variable]:kor-pp-ro:}<br/>
    /// <br/>
    /// result:<br/>
    /// i: {기차:kor-pp-ro:}<br/>
    /// o: 기차로<br/>
    /// <br/>
    /// i: {집:kor-pp-ro:}<br/>
    /// o: 집으로<br/>
    /// <br/>
    /// i: {not-korean:kor-pp-ro:}<br/>
    /// o: not-korean(으)로<br/>
    /// </summary>
    [UsedImplicitly]
    public class KoreanPostPositionRoFormatter : FormatterBase
    {
        public override string[] DefaultNames => new string[] { "kor-pp-ro" };

        private static readonly string RO = "로";
        private static readonly string EURO = "으로";
        private static readonly string N_A = "(으)로";

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            string str = formattingInfo.CurrentValue.ToString();
            formattingInfo.Write(str);

            if (str.Length == 0)
            {
                formattingInfo.Write(N_A);
                return true;
            }

            formattingInfo.Write(formattingInfo.Format.RawText);

            char lastLetter = str[^1];
            if (lastLetter >= '가' && lastLetter <= '힣')
            {
                int lastConsonantIndex = (lastLetter - '가') % 28;
                if (lastConsonantIndex == 0 || lastConsonantIndex == ('ㄹ' - 'ㄱ'))
                {
                    formattingInfo.Write(RO);
                }
                else
                {
                    formattingInfo.Write(EURO);
                }
            }
            else
            {
                formattingInfo.Write(N_A);
            }

            return true;
        }
    }
}
