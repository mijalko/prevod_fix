Add-Type -AssemblyName System.Drawing

function New-IconBitmap([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)

    # Dark blue background
    $bgBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 18, 52, 120))
    $g.FillRectangle($bgBrush, 0, 0, $size, $size)

    # Two subtitle bars at bottom
    $white = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $mx  = [int]($size * 0.10)
    $lh  = [Math]::Max(2, [int]($size * 0.12))
    $gap = [Math]::Max(1, [int]($size * 0.06))
    $y2  = [int]($size * 0.87) - $lh
    $y1  = $y2 - $lh - $gap
    $w1  = $size - 2 * $mx
    $w2  = [int]($w1 * 0.62)
    $g.FillRectangle($white, $mx, $y1, $w1, $lh)
    $g.FillRectangle($white, $mx, $y2, $w2, $lh)

    # Clock circle
    $cr  = [int]($size * 0.24)
    $cx  = [int]($size * 0.50)
    $cy  = [int]($size * 0.33)
    $pw  = [float][Math]::Max(1.5, $size * 0.07)
    $clockPen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, $pw)
    $g.DrawEllipse($clockPen, $cx - $cr, $cy - $cr, $cr * 2, $cr * 2)

    # Clock hands
    $handPen = New-Object System.Drawing.Pen([System.Drawing.Color]::White, [float][Math]::Max(1.0, $size * 0.055))
    $handPen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $handPen.EndCap   = [System.Drawing.Drawing2D.LineCap]::Round
    # Minute hand — pointing to 12
    $g.DrawLine($handPen, $cx, $cy, $cx, $cy - [int]($cr * 0.80))
    # Hour hand — pointing to ~2
    $g.DrawLine($handPen, $cx, $cy, $cx + [int]($cr * 0.58), $cy - [int]($cr * 0.38))

    $g.Dispose()
    foreach ($obj in @($bgBrush, $white, $clockPen, $handPen)) { $obj.Dispose() }
    return $bmp
}

function Save-Ico([string]$path, [int[]]$sizes) {
    $streams = @()
    foreach ($s in $sizes) {
        $bmp = New-IconBitmap $s
        $ms  = New-Object System.IO.MemoryStream
        $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
        $streams += $ms
    }

    $count      = $streams.Count
    $dataOffset = 6 + $count * 16

    $fs = [System.IO.File]::Create($path)
    $w  = New-Object System.IO.BinaryWriter($fs)

    # ICO header
    $w.Write([uint16]0)
    $w.Write([uint16]1)
    $w.Write([uint16]$count)

    # Directory entries
    $offset = $dataOffset
    for ($i = 0; $i -lt $count; $i++) {
        $s = $sizes[$i]
        $w.Write([byte]$(if ($s -eq 256) { 0 } else { $s }))
        $w.Write([byte]$(if ($s -eq 256) { 0 } else { $s }))
        $w.Write([byte]0)
        $w.Write([byte]0)
        $w.Write([uint16]1)
        $w.Write([uint16]32)
        $w.Write([uint32]$streams[$i].Length)
        $w.Write([uint32]$offset)
        $offset += $streams[$i].Length
    }

    # PNG image data
    foreach ($ms in $streams) {
        $w.Write($ms.ToArray())
        $ms.Dispose()
    }
    $w.Close()
    $fs.Close()
}

Save-Ico "C:\mijalko\projects\Prevod\Prevod\app.ico" @(16, 32, 48, 256)
Write-Host "Icon saved to app.ico"
