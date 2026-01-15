import { create } from 'zustand';

interface TodoStoreState {
  selectedListId: string | null;
  setSelectedListId: (id: string | null) => void;
}

export const useTodoStore = create<TodoStoreState>((set) => ({
  selectedListId: null,
  setSelectedListId: (id) => set({ selectedListId: id }),
}));
