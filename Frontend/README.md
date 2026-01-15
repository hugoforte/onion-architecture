# Todo App Frontend

React 18 + TypeScript + Vite + Material UI frontend for the Todo App template.

## Tech Stack

- **React 18** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **Material UI (MUI)** - Component library
- **TanStack Query** - Data fetching and caching
- **Zustand** - State management
- **Axios** - HTTP client

## Setup

### Prerequisites

- Node.js 22+ and npm

### Installation

```bash
cd Frontend
npm install
```

### Development

```bash
npm run dev
```

The application will be available at `http://localhost:5173` and will proxy API calls to `http://localhost:5003`.

### Build

```bash
npm run build
```

### Linting

```bash
npm run lint
npm run format
```

## Project Structure

```
src/
├── main.tsx                 # Entry point
├── App.tsx                  # Root component
├── index.css               # Global styles
├── api/
│   ├── client.ts           # Axios instance
│   └── todoApi.ts          # API endpoints
├── store/
│   └── todoStore.ts        # Zustand store
└── features/
    ├── todoLists/
    │   └── TodoListsView.tsx
    └── todoItems/
        └── TodoItemsView.tsx
```

## Environment Variables

Create a `.env` file in the Frontend directory:

```
VITE_API_BASE_URL=http://localhost:5003
```

## API Integration

The frontend communicates with the backend API at `/api`:

- `GET /api/todo-lists` - Get all todo lists
- `POST /api/todo-lists` - Create a new todo list
- `DELETE /api/todo-lists/{id}` - Delete a todo list
- `GET /api/todo-lists/{id}/items` - Get items for a list
- `POST /api/todo-items` - Create a new todo item
- `PUT /api/todo-items/{id}/complete` - Mark item as complete
- `DELETE /api/todo-items/{id}` - Delete a todo item

## Features

- Create and manage todo lists
- Add items to lists
- Mark items as complete
- Delete lists and items
- Responsive Material UI design
- Real-time API integration with TanStack Query

## Contributing

Follow the code style enforced by ESLint and Prettier.
