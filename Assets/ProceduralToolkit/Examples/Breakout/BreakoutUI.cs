﻿using UnityEngine;

namespace ProceduralToolkit.Examples.UI
{
    public class BreakoutUI : UIBase
    {
        public RectTransform leftPanel;

        private int wallWidth = 9;
        private int wallHeight = 7;
        private int wallHeightOffset = 5;
        private float paddleWidth = 1;
        private float ballSize = 0.5f;
        private float ballVelocityMagnitude = 5;

        private Breakout breakout;

        private void Awake()
        {
            breakout = new Breakout();
            Generate();

            var instructionsText = InstantiateControl<TextControl>(leftPanel);
            instructionsText.Initialize("Use A/D or Left/Right to move");

            var wallWidthSlider = InstantiateControl<SliderControl>(leftPanel);
            wallWidthSlider.Initialize("Wall width", 1, 20, wallWidth, value =>
            {
                wallWidth = value;
                Generate();
            });

            var wallHeightSlider = InstantiateControl<SliderControl>(leftPanel);
            wallHeightSlider.Initialize("Wall height", 1, 20, wallHeight, value =>
            {
                wallHeight = value;
                Generate();
            });

            var wallHeightOffsetSlider = InstantiateControl<SliderControl>(leftPanel);
            wallHeightOffsetSlider.Initialize("Wall height offset", 1, 10, wallHeightOffset, value =>
            {
                wallHeightOffset = value;
                Generate();
            });

            var paddleWidthSlider = InstantiateControl<SliderControl>(leftPanel);
            paddleWidthSlider.Initialize("Paddle width", 1, 10, paddleWidth, value =>
            {
                paddleWidth = value;
                Generate();
            });

            var ballSizeSlider = InstantiateControl<SliderControl>(leftPanel);
            ballSizeSlider.Initialize("Ball size", 0.5f, 3f, ballSize, value =>
            {
                ballSize = value;
                Generate();
            });

            var ballVelocitySlider = InstantiateControl<SliderControl>(leftPanel);
            ballVelocitySlider.Initialize("Ball velocity", 1, 20, ballVelocityMagnitude, value =>
            {
                ballVelocityMagnitude = value;
                Generate();
            });

            var generateButton = InstantiateControl<ButtonControl>(leftPanel);
            generateButton.Initialize("Generate", Generate);
        }

        private void Generate()
        {
            breakout.UpdateParameters(wallWidth, wallHeight, wallHeightOffset, paddleWidth, ballSize,
                ballVelocityMagnitude);
            breakout.ResetLevel();
        }

        private void Update()
        {
            breakout.Update();
        }
    }
}