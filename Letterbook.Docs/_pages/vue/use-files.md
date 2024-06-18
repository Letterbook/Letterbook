---
title: File Utils
group: Library
---

The file utils are utilized by the `<FileInput>` [Input component](/vue/form-inputs) and 
`icon`, `iconRounded` and `attachment` [formatters](/vue/use-formatters) for resolving file SVG Icons 
and MIME Types that Apps can also utilize in `useFiles()`

```js
import { useFiles } from '@servicestack/vue'

const {
    extSvg,           // Resolve SVG XML for file extension
    extSrc,           // Resolve SVG Data URI for file extension
    getExt,           // Resolve File extension from file name or path
    canPreview,       // Check if path or URI is of a supported web image type
    getFileName,      // Resolve file name from /file/path
    getMimeType,      // Resolve the MIME type for a file path name or extension
    formatBytes,      // Format file size in human readable bytes
    filePathUri,      // Resolve the Icon URI to use for file
    encodeSvg,        // Encode SVG XML for usage in Data URIs
    svgToDataUri,     // Convert SVG XML to data:image URL
    fileImageUri,     // Resolve image preview URL for file
    objectUrl,        // Create and track an Object URL for an uploaded file
    flush,            // Release all tracked Object URLs
    inputFiles,       // Resolve file metadata for all uploaded HTML input files
    iconOnError,      // Error handler for broken images to return a fallbackSrc
    iconFallbackSrc,  // Resolve the fallback URL for a broken Image URL
} = useFiles()
```

## TypeScript Definition

TypeScript definition of the API surface area and type information for correct usage of `useFiles()`

```ts
/** Resolve SVG XML for file extension */
function extSvg(ext: string): string | null;

/** Resolve SVG URI for file extension */
function extSrc(ext: string): any;

/** Resolve File extension from file name or path */
function getExt(path?: string | null): string | null;

/** Check if path or URI is of a supported web image type */
function canPreview(path: string): boolean;

/** Resolve file name from /file/path */
function getFileName(path?: string | null): string | null;

/** Resolve the MIME type for a file path name or extension */
function getMimeType(fileNameOrExt: string): string;

/** Format file size in human readable bytes */
function formatBytes(bytes: number, d?: number): string;

/** Resolve the Icon URI to use for file */
function filePathUri(path?: string): string | null;

/** Encode SVG XML for usage in Data URIs */
function encodeSvg(s: string): string;

/** Convert SVG XML to data:image URL */
function svgToDataUri(svg: string): string;

/** Resolve image preview URL for file */
function fileImageUri(file: any | {
    name: string;
}): string | null;

/** Create and track Image URL for an uploaded file */
function objectUrl(file: Blob | MediaSource): string;

/** Release all tracked Object URLs */
function flush(): void;

/** Resolve file metadata for all uploaded HTML file input files */
function inputFiles(input: HTMLInputElement): {
    fileName: string;
    contentLength: number;
    filePath: string | null;
}[] | null;

/** Error handler for broken images to return a fallbackSrc */
function iconOnError(img: HTMLImageElement, fallbackSrc?: string): void;

/** Resolve the fallback URL for a broken Image URL */
function iconFallbackSrc(src: string, fallbackSrc?: string): string | null;
```