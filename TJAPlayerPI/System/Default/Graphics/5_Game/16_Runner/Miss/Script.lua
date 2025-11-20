local tau = 2.0 * math.pi

Runner = {}
Runner.Variant = 1
Runner.AnimSpeed = 1

local config = Content:LoadJson("Config.json")
local in_pos_length = config.in_pos_length
local in_beat_length = config.in_beat_length
local loop_beat_length = config.loop_beat_length
local loop_wave = config.loop_wave

local tx_in = Content:LoadTexture("In.png")
local tx_loop = Content:LoadTexture("Loop.png")

local config_x = config.x
local config_y = config.y
local config_loop_x = config.loop_x
local config_loop_y = config.loop_y
local config_width = config.width
local config_height = config.height

function runnerWave(value)
    if value < 0.5 then
        return math.sin(value * math.pi)
    else
        local wave_value = value - 0.5
        return (math.cos(wave_value * tau) * 0.5) + 0.5
    end
end

function draw(value, ptn, player)
    local lua_index = player + 1

    local beat = value * 4

    if beat < in_beat_length then
        local x = config_x[lua_index]
        local y = config_y[lua_index]
        local in_value = MathEx:InvLerp(0.0, in_beat_length, beat)
        x = x + (in_value * in_pos_length)

        local frame
        if in_value < 0.25 then
            frame = 0
        else
            frame = 1
        end

        tx_in.opacity = in_value
        tx_in:drawRect(x, y, config_width * frame, config_height * ptn, config_width, config_height)
    else
        local x = config_loop_x[lua_index]
        local y = config_loop_y[lua_index]
        local loop_value = beat - in_beat_length
        x = x + (loop_value * loop_beat_length)

        local wave = runnerWave(loop_value)
        y = y + (wave * loop_wave)

        local tex
        local frame
        if loop_value < 0.25 then
            tex = tx_in
            frame = 2
        else
            tex = tx_loop

            local state = loop_value - 0.5
            state = state - math.floor(state)
            if state < 0.5 then
                frame = 0
            else
                frame = 1
            end
        end

        tex.opacity = 1.0
        tex:drawRect(x, y, config_width * frame, config_height * ptn, config_width, config_height)
    end

end
