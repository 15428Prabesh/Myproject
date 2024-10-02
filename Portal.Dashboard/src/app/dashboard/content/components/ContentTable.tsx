import BaseTable from "@/app/components/table/BaseTable";
import { ScriptProps } from "next/script";
import { Box } from "@chakra-ui/react";

const style={
    borderRadius:'10px',
    paddingTop:'25px',
    paddingBottom:'25px'
}

export default function ContentDataTable({props}:{props?:ScriptProps})
{
    return(
        <Box className='content-table'>
            <BaseTable style={style}/>
        </Box>
    )
}