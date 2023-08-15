import { Home } from "./components/Home";
import { OfficeTest } from "./components/OfficeTest";

const AppRoutes = [
  {
    index: true,
    element: <Home />
    },
    {
        path: '/Office',
        element: <OfficeTest />
    },
];

export default AppRoutes;
