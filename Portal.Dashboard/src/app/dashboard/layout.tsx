
import { Box, Grid,GridItem } from "@chakra-ui/react"
import React from "react"
import Navbar from "../components/navbar/Navbar"
import Sidebar from "../components/sidebar/Sidebar"

const AdminLayout=({children}:{children:React.ReactNode})=>{

    return(
        <Box bg='darkBackground.800'>
            <Grid
            templateAreas={`"header header"
                            "nav main"
                            "nav footer"`}
            gridTemplateColumns={'240px 1fr'}
            color='blackAlpha.700'
            fontWeight='bold'

            >
            <GridItem   area={'nav'}>
                <Sidebar/>
            </GridItem>
            <GridItem  area={'main'}>
                <Navbar/>
                {children}
            </GridItem>
            </Grid>
        </Box>

    )
}

export default AdminLayout