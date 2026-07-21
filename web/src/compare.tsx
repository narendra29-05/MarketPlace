import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from "react";

export interface CompareItem {
  id: number;
  name: string;
}

interface CompareContextValue {
  items: CompareItem[];
  toggle: (item: CompareItem) => void;
  remove: (id: number) => void;
  clear: () => void;
  has: (id: number) => boolean;
}

const CompareContext = createContext<CompareContextValue | null>(null);

const MAX_COMPARE = 4;

export function CompareProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CompareItem[]>([]);

  const toggle = useCallback((item: CompareItem) => {
    setItems(current => {
      if (current.some(existing => existing.id === item.id)) {
        return current.filter(existing => existing.id !== item.id);
      }
      if (current.length >= MAX_COMPARE) return current;
      return [...current, item];
    });
  }, []);

  const remove = useCallback((id: number) => {
    setItems(current => current.filter(item => item.id !== id));
  }, []);

  const clear = useCallback(() => setItems([]), []);

  const value = useMemo(
    () => ({
      items,
      toggle,
      remove,
      clear,
      has: (id: number) => items.some(item => item.id === id),
    }),
    [items, toggle, remove, clear],
  );

  return <CompareContext.Provider value={value}>{children}</CompareContext.Provider>;
}

export function useCompare(): CompareContextValue {
  const context = useContext(CompareContext);
  if (!context) throw new Error("useCompare must be used inside CompareProvider");
  return context;
}
