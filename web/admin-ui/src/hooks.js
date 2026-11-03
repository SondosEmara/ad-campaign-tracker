import { useCallback, useEffect, useState } from "react";

/** Loads data once and exposes reload(). Keeps old data visible while refreshing. */
export function useAsync(loader) {
  const [state, setState] = useState({ data: null, error: null, loading: true });

  const reload = useCallback(async () => {
    setState((s) => ({ ...s, loading: true }));
    try {
      setState({ data: await loader(), error: null, loading: false });
    } catch (error) {
      setState({ data: null, error, loading: false });
    }
  }, [loader]);

  useEffect(() => {
    reload();
  }, [reload]);

  return { ...state, reload };
}
