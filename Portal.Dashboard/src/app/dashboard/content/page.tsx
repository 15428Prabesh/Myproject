import { Text, Grid, GridItem } from "@chakra-ui/react";
import ContentDataTable from "./components/ContentTable";
import mockData from "@/app/moke/Data";
import IContent from "@/app/interface/IContent";
import Link from "next/link";

const link_style = {
  backgroundColor: "#3182CE",
  padding: 10,
  borderRadius: 5,
  color: 'white',
};

const ContentPage = () => {
  const Datatable: IContent[] = mockData;

  return (
    <Grid padding={10}>
        <GridItem marginY={5}>
            <Grid templateColumns="1fr 1fr" alignItems="center"  >
              <GridItem>
                  <Text fontSize="30" color="white">
                  Contents
                  </Text>
              </GridItem>
              <GridItem justifySelf='end'>
                  <Link style={link_style} href="/dashboard/content/create">
                  Add Content
                  </Link>
              </GridItem>
            </Grid>
        </GridItem>
        <GridItem>
            <ContentDataTable />
        </GridItem>
    </Grid>
  );
};

export default ContentPage;
