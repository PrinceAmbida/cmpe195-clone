"""Add the two-level browser page and package a completed Unity Web build."""
from pathlib import Path
import json
import shutil
import zipfile

ROOT = Path(__file__).resolve().parent.parent
BUILD = ROOT / 'Exports' / 'WebGL'
OUTPUT = ROOT / 'Exports' / 'TransportNetworkLayers-WebGL.zip'

def one_file(pattern):
    matches = sorted((BUILD / 'Build').glob(pattern))
    if len(matches) != 1:
        raise RuntimeError(f'Expected one {pattern}, found {matches}')
    return matches[0].relative_to(BUILD).as_posix()

def main():
    replacements = {
        '__LOADER_URL__': one_file('*.loader.js'),
        '__DATA_URL__': one_file('*.data*'),
        '__FRAMEWORK_URL__': one_file('*.framework.js*'),
        '__CODE_URL__': one_file('*.wasm*'),
    }
    page = (ROOT / 'WebBuild' / 'index.html').read_text(encoding='utf-8')
    for marker, path in replacements.items():
        page = page.replace(marker, path)
    (BUILD / 'index.html').write_text(page, encoding='utf-8')
    shutil.copyfile(ROOT / 'Assets/Resources/LayerMusic/CREDITS.txt', BUILD / 'MUSIC-CREDITS.txt')
    (BUILD / 'build-info.json').write_text(json.dumps({
        'unity_version': '6000.5.4f1',
        'resolution': {'width': 1200, 'height': 700},
        'music_volume': 0.15,
        'scenes': ['TransportLayer', 'NetworkLayer'],
        'compression': 'gzip with Unity decompression fallback',
    }, indent=2), encoding='utf-8')
    with zipfile.ZipFile(OUTPUT, 'w', compression=zipfile.ZIP_DEFLATED) as archive:
        for path in sorted(BUILD.rglob('*')):
            if path.is_file() and path.name != 'unity-build.provenance.json':
                archive.write(path, path.relative_to(BUILD).as_posix())
    print(f'Created {OUTPUT} ({OUTPUT.stat().st_size:,} bytes)')

if __name__ == '__main__':
    main()
