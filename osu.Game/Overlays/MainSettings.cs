﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Overlays.Settings;
using osu.Game.Overlays.Settings.Sections;
using osu.Game.Screens.Ranking;
using OpenTK;

namespace osu.Game.Overlays
{
    public class MainSettings : SettingsOverlay
    {
        private readonly KeyBindingOverlay keyBindingOverlay;
        private BackButton backButton;

        protected override IEnumerable<SettingsSection> CreateSections() => new SettingsSection[]
        {
            new GeneralSection(),
            new GraphicsSection(),
            new GameplaySection(),
            new AudioSection(),
            new SkinSection(),
            new InputSection(keyBindingOverlay),
            new OnlineSection(),
            new MaintenanceSection(),
            new DebugSection(),
        };

        protected override Drawable CreateHeader() => new SettingsHeader("settings", "Change the way osu! behaves");
        protected override Drawable CreateFooter() => new SettingsFooter();

        public MainSettings()
            : base(true)
        {
            keyBindingOverlay = new KeyBindingOverlay { Depth = 1 };
            keyBindingOverlay.StateChanged += keyBindingOverlay_StateChanged;
        }

        public override bool AcceptsFocus => keyBindingOverlay.State != Visibility.Visible;

        private void keyBindingOverlay_StateChanged(VisibilityContainer container, Visibility visibility)
        {
            const float hidden_width = 120;

            switch (visibility)
            {
                case Visibility.Visible:
                    Background.FadeTo(0.9f, 500, Easing.OutQuint);
                    SectionsContainer.FadeOut(100);
                    ContentContainer.MoveToX(hidden_width - ContentContainer.DrawWidth, 500, Easing.OutQuint);

                    backButton.Delay(100).FadeIn(100);
                    break;
                case Visibility.Hidden:
                    Background.FadeTo(0.6f, 500, Easing.OutQuint);
                    SectionsContainer.FadeIn(500, Easing.OutQuint);
                    ContentContainer.MoveToX(0, 500, Easing.OutQuint);

                    backButton.FadeOut(100);
                    break;
            }
        }

        protected override void PopOut()
        {
            base.PopOut();
            keyBindingOverlay.State = Visibility.Hidden;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddInternal(keyBindingOverlay);
            AddInternal(backButton = new BackButton
            {
                Alpha = 0,
                Height = 150,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Action = () => keyBindingOverlay.Hide()
            });
        }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            keyBindingOverlay.Margin = new MarginPadding { Left = ContentContainer.Margin.Left + ContentContainer.DrawWidth + ContentContainer.X };

            backButton.Margin = new MarginPadding { Left = ContentContainer.Margin.Left };
            backButton.Width = ContentContainer.DrawWidth + ContentContainer.X;
        }

        private class BackButton : OsuClickableContainer
        {
            private FillFlowContainer flow;
            private AspectContainer aspect;

            [BackgroundDependencyLoader]
            private void load()
            {
                Children = new Drawable[]
                {
                    aspect = new AspectContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Y,
                        Children = new Drawable[]
                        {
                            flow = new FillFlowContainer
                            {
                                Anchor = Anchor.TopCentre,
                                RelativePositionAxes = Axes.Y,
                                Y = 0.4f,
                                AutoSizeAxes = Axes.Both,
                                Origin = Anchor.Centre,
                                Direction = FillDirection.Horizontal,
                                Children = new[]
                                {
                                    new SpriteIcon { Size = new Vector2(15), Shadow = true, Icon = FontAwesome.fa_chevron_left },
                                    new SpriteIcon { Size = new Vector2(15), Shadow = true, Icon = FontAwesome.fa_chevron_left },
                                    new SpriteIcon { Size = new Vector2(15), Shadow = true, Icon = FontAwesome.fa_chevron_left },
                                }
                            },
                            new OsuSpriteText
                            {
                                Anchor = Anchor.TopCentre,
                                RelativePositionAxes = Axes.Y,
                                Y = 0.7f,
                                TextSize = 12,
                                Font = @"Exo2.0-Bold",
                                Origin = Anchor.Centre,
                                Text = @"back",
                            },
                        }
                    }
                };
            }

            protected override bool OnHover(InputState state)
            {
                flow.TransformSpacingTo(new Vector2(5), 500, Easing.OutQuint);
                return base.OnHover(state);
            }

            protected override void OnHoverLost(InputState state)
            {
                flow.TransformSpacingTo(new Vector2(0), 500, Easing.OutQuint);
                base.OnHoverLost(state);
            }

            protected override bool OnMouseDown(InputState state, MouseDownEventArgs args)
            {
                aspect.ScaleTo(0.75f, 2000, Easing.OutQuint);
                return base.OnMouseDown(state, args);
            }

            protected override bool OnMouseUp(InputState state, MouseUpEventArgs args)
            {
                aspect.ScaleTo(1, 1000, Easing.OutElastic);
                return base.OnMouseUp(state, args);
            }
        }
    }
}