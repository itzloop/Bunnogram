using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bunnogram
{
    public class Icon : VisualElement
    {
        
        Symbol _symbol;
        Label _symbolLabel;        
        
        public enum Symbol
        {
            LightBulb,
        }

        private static readonly Dictionary<Symbol, char> _symbolGlyphMap = new Dictionary<Symbol, char>
        {
            { Symbol.LightBulb, '\uf0eb' }
        };
        
        public Symbol symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                _symbolLabel.text = _symbolGlyphMap[value].ToString();
            }
        } 
        
        public Icon() : this(Symbol.LightBulb) {}
        
        public Icon(Symbol symbol)
        {
            _symbolLabel = new Label
            {
                style =
                {
                    unityFont = Resources.Load<Font>("Fonts/fa-solid-900"),
                    unityFontStyleAndWeight = StyleKeyword.None,
                },
            }; 
            
            Add(_symbolLabel);
            this.symbol = symbol;
        }
        public new class UxmlFactory : UxmlFactory<Icon, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            readonly UxmlEnumAttributeDescription<Symbol> _symbolAttribute = new UxmlEnumAttributeDescription<Symbol>() { name = "symbol" };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var icon = (Icon)ve;
                icon.symbol = _symbolAttribute.GetValueFromBag(bag, cc);
            }
        } 
    }
}