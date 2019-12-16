using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BlazorTable
{
    public partial class Popover
    {
        [Parameter(CaptureUnmatchedValues = true)]
        public IReadOnlyDictionary<string, object> UnknownParameters { get; set; }

        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

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

        [Parameter]
        public Placement Placement { get; set; } = Placement.Auto;

        [Parameter]
        public ElementReference Reference { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool DismissOnNextClick { get; set; } = true;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        public bool Manual { get; set; }

        private bool _isOpen { get; set; }

        protected virtual Task Changed(bool e)
        {
            return Task.CompletedTask;
        }

        public virtual void Hide()
        {
            _isOpen = false;
            if (!Manual) Changed(_isOpen);
            IsOpenChanged.InvokeAsync(false);
        }

        protected string Classname => $"popover bs-popover-{Placement.ToDescriptionString()} {(IsOpen == true ? "show" : string.Empty)}";

        protected ElementReference MyRef { get; set; }

        protected ElementReference Arrow { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            if (IsOpen ?? false)
            {
                var placement = Placement.ToDescriptionString();
                JSRuntime.InvokeVoidAsync("BlazorTablePopper", Reference, MyRef, Arrow, placement);
            }
        }

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
