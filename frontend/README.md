# Phonebook frontend

Vue 3 single-page application for the Phonebook Application. It uses Vite for development and production builds and calls the existing `/api/contacts` endpoints.

## Requirements

- Node `^20.19.0 || >=22.12.0`
- The backend running at `https://localhost:7233` for the development proxy

## Commands

```text
npm ci
npm run dev
npm run build
npm run preview
```

`npm run build` writes the static application to `dist/`.

## API proxy

`vite.config.js` proxies requests beginning with `/api` to `https://localhost:7233` with `changeOrigin: true` and `secure: false` for local development. The frontend uses relative `/api/contacts` URLs, so the browser sees same-origin requests.

The preview command does not inherit the development proxy. In production, serve `dist/` separately and route `/api/*` to the backend through a same-origin reverse proxy, or configure an explicit cross-origin API policy outside this package.
