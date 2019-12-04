using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BlazorTable
{
    public partial class Popover
    {
        internal ElementReference MyRef { get; set; }

        [Parameter] 
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter] 
        public string AnimationClass { get; set; }

        [Parameter]
        public bool? IsOpen
        {
            get => _isOpen;
            set
            {
                if (value != null)
                {
                    Manual = true;
                    if (value.Value != _isOpen)
                    {
                        Changed(value ?? false);
                        StateHasChanged();
                    }
                    _isOpen = value ?? false;
                }
            }
        }

        public bool Manual { get; set; } = false;

        private bool _isOpen { get; set; }

        internal virtual Task Changed(bool e)
        {
            return Task.CompletedTask;
        }

        public virtual void Show()
        {
            _isOpen = true;
            if (!Manual) Changed(_isOpen);
            IsOpenChanged.InvokeAsync(true);
        }

        public virtual void Hide()
        {
            _isOpen = false;
            if (!Manual) Changed(_isOpen);
            IsOpenChanged.InvokeAsync(false);
        }

        public virtual void Toggle()
        {
            _isOpen = !_isOpen;
            if (!Manual) Changed(_isOpen);
            IsOpenChanged.InvokeAsync(_isOpen);
        }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> UnknownParameters { get; set; }
        
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected string Classname => $"popover bs-popover-{Placement.ToDescriptionString()} {(IsOpen == true ? "show" : string.Empty)}";

        protected ElementReference Arrow { get; set; }

        protected override void OnAfterRender(bool firstrun)
        {
            if (IsOpen ?? false)
            {
                var placement = Placement.ToDescriptionString();
                JSRuntime.InvokeVoidAsync("BlazorTablePopper", Target, Id, Arrow, placement);
            }
        }

        protected string Id => Target + "-popover";

        [Parameter] public Placement Placement { get; set; } = Placement.Auto;
        [Parameter] public string Target { get; set; }
        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool DismissOnNextClick { get; set; } = true;

        protected void OnClick()
        {
            if (DismissOnNextClick)
            {
                Hide();
            }
        }
    }

    public enum Placement
    {
        [Description("auto")]
        Auto,
        [Description("auto-start")]
        AutoStart,
        [Description("auto-end")]
        AutoEnd,
        [Description("top")]
        Top,
        [Description("top-start")]
        TopStart,
        [Description("top-end")]
        TopEnd,
        [Description("right")]
        Right,
        [Description("right-start")]
        RightStart,
        [Description("right-end")]
        EightEnd,
        [Description("bottom")]
        Bottom,
        [Description("bottom-start")]
        BottomStart,
        [Description("bottom-end")]
        BottomEnd,
        [Description("left")]
        Left,
        [Description("left-start")]
        LeftStart,
        [Description("left-end")]
        LeftEnd
    }
}
