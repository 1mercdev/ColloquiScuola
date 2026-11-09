using System;
using System.Collections.Generic;
using System.Text;

namespace SchoolMeetings.Mobile.Behaviors
{

    // WIP (has issues)

    public class DismissKeyboardBehavior : Behavior<View>
    {
        private TapGestureRecognizer? _tapGesture;

        protected override void OnAttachedTo(View bindable)
        {
            base.OnAttachedTo(bindable);

            _tapGesture = new TapGestureRecognizer();
            _tapGesture.Tapped += (sender, e) =>
            {
                bindable.Window?.Page?.Unfocus();
            };

            bindable.GestureRecognizers.Add(_tapGesture);
        }

        protected override void OnDetachingFrom(View bindable)
        {
            base.OnDetachingFrom(bindable);

            if (_tapGesture != null)
            {
                bindable.GestureRecognizers.Remove(_tapGesture);
                _tapGesture = null;
            }
        }
    }
}
