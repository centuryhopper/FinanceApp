import { useEffect, useState } from "react";

export default function useCountDown({
  timesUpCallback,
  repeatedCallback,
}: {
  timesUpCallback?: () => void;
  repeatedCallback?: () => void;
}) {
  
  const [secondsLeft, setSecondsLeft] = useState(0);
  useEffect(() => {
    if (secondsLeft <= 0) {
      timesUpCallback && timesUpCallback();
      return;
    }

    repeatedCallback && repeatedCallback();

    // delay one second
    const timeout = setTimeout(function () {
      setSecondsLeft(secondsLeft - 1);
      console.log("secondsLeft: " + secondsLeft);
    }, 1000);

    // clean up
    return () => clearTimeout(timeout);
  }, [secondsLeft]);

  const startTimer = (seconds: number) => {
    setSecondsLeft(seconds);
  };

  return {
    secondsLeft,
    startTimer,
  };
}
