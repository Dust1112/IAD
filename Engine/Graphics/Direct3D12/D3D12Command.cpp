#pragma once
#include "D3D12CommonHeaders.hpp"
#include "D3D12Core.hpp"

namespace iad::graphics::d3d12::core
{
    class dxd12_command
    {
    public:
        explicit dxd12_command(ID3D12Device8 *const device, D3D12_COMMAND_LIST_TYPE type)
        {
            HRESULT hr{ S_OK };
    
            D3D12_COMMAND_QUEUE_DESC desc{};
            desc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
            desc.NodeMask = 0;
            desc.Priority = D3D12_COMMAND_QUEUE_PRIORITY_NORMAL;
            desc.Type = type;
        
            DX_CALL(hr = device->CreateCommandQueue(&desc, IID_PPV_ARGS(&_cmd_queue)));
            if (FAILED(hr)) { goto _error; }
            NAME_D3D12_OBJECT(_cmd_queue,
                type == D3D12_COMMAND_LIST_TYPE_DIRECT
                ? L"GFX Command Queue"
                : type == D3D12_COMMAND_LIST_TYPE_DIRECT
                    ? L"Compute Command Queue" : L"Command Queue");

            for (u32 i{ 0 }; i <frame_buffer_count; ++i)
            {
                command_frame& frame{ _cmd_frames[i] };
                DX_CALL(hr = device->CreateCommandAllocator(type, IID_PPV_ARGS(&frame.cmd_allocator)));
                if (FAILED(hr)) { goto _error; }

                NAME_D3D12_OBJECT_INDEXED(frame.cmd_allocator, i,
                type == D3D12_COMMAND_LIST_TYPE_DIRECT
                ? L"GFX Command Allocator"
                : type == D3D12_COMMAND_LIST_TYPE_DIRECT
                    ? L"Compute Command Allocator" : L"Command Allocator");
            }
            
            DX_CALL(hr = device->CreateCommandList(0, type, _cmd_frames[0].cmd_allocator, nullptr, IID_PPV_ARGS(&_cmd_list)));
            if(FAILED(hr)) { goto _error; }
            DX_CALL(_cmd_list->Close());

            NAME_D3D12_OBJECT(_cmd_list,
                type == D3D12_COMMAND_LIST_TYPE_DIRECT
                ? L"GFX Command List"
                : type == D3D12_COMMAND_LIST_TYPE_DIRECT
                    ? L"Compute Command List" : L"Command List");
        
            _error:
                release();
        }

        void release()
        {
        
        }

        void begin_frame()
        {
            command_frame& frame{ _cmd_frames[_frame_index] };
            frame.wait();
            DX_CALL(frame.cmd_allocator->Reset());
            DX_CALL(_cmd_list->Reset(frame.cmd_allocator, nullptr));
        }

        void end_frame()
        {
            DX_CALL(_cmd_list->Close());
            ID3D12CommandList *const cmd_lists[]{ _cmd_list };
            _cmd_queue->ExecuteCommandLists(_countof(cmd_lists), &cmd_lists[0]);
            
            _frame_index = (_frame_index + 1) % frame_buffer_count;
        }

    private:
        struct command_frame
        {
            ID3D12CommandAllocator* cmd_allocator{ nullptr };

            void wait()
            {
                
            }

            void release()
            {
                core::release(cmd_allocator);
            }
        };
    
        ID3D12CommandQueue* _cmd_queue{ nullptr };
        ID3D12GraphicsCommandList6* _cmd_list{ nullptr };
        command_frame _cmd_frames[frame_buffer_count]{};
        u32 _frame_index{ 0 };
    };
}