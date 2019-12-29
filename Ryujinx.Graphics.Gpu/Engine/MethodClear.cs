using Ryujinx.Graphics.GAL;
using Ryujinx.Graphics.Gpu.State;

namespace Ryujinx.Graphics.Gpu.Engine
{
    partial class Methods
    {
        private void Clear(GpuState state, int argument)
        {
            UpdateRenderTargetState(state, useControl: false);

            TextureManager.CommitGraphicsBindings();

            bool clearDepth   = (argument & 1) != 0;
            bool clearStencil = (argument & 2) != 0;

            uint componentMask = (uint)((argument >> 2) & 0xf);

            int index = (argument >> 6) & 0xf;

            if (componentMask != 0)
            {
                var clearColor = state.Get<ClearColors>(MethodOffset.ClearColors);

                ColorF color = new ColorF(
                    clearColor.Red,
                    clearColor.Green,
                    clearColor.Blue,
                    clearColor.Alpha);

                _context.Renderer.Pipeline.ClearRenderTargetColor(index, componentMask, color);
            }

            if (clearDepth || clearStencil)
            {
                float depthValue   = state.Get<float>(MethodOffset.ClearDepthValue);
                int   stencilValue = state.Get<int>  (MethodOffset.ClearStencilValue);

                int stencilMask = 0;

                if (clearStencil)
                {
                    stencilMask = state.Get<StencilTestState>(MethodOffset.StencilTestState).FrontMask;
                }

                _context.Renderer.Pipeline.ClearRenderTargetDepthStencil(
                    depthValue,
                    clearDepth,
                    stencilValue,
                    stencilMask);
            }

            UpdateRenderTargetState(state, useControl: true);
        }
    }
}