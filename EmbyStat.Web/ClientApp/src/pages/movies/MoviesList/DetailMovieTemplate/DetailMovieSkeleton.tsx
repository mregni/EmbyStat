import React from 'react'
import { Grid } from '@material-ui/core';
import { Skeleton } from '@material-ui/lab';
interface Props {

}

const DetailMovieSkeleton = (props: Props) => {
  return (
    <Grid container>
      <Grid item className="m-r-16">
        <Skeleton variant="rect" width={200} height={300} animation="wave" className="m-l-8 m-r-8" />
      </Grid>
      <Grid item container direction="column" xs spacing={1} justify="flex-start">
        <Grid item container spacing={2} alignItems="center">
          <Grid item>
            <Skeleton animation="wave" variant="text" width={450} height={50} />
          </Grid>
          <Grid item>
            <Skeleton animation="wave" variant="text" width={150} height={50} />
          </Grid>
        </Grid>
        <Grid item>
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
        <Grid item container alignItems="center">
          <Skeleton animation="wave" variant="circle" width={20} height={20} className="m-r-8" />
          <Skeleton animation="wave" variant="text" width={200} height={20} />
        </Grid>
      </Grid>
    </Grid>
  )
}

export default DetailMovieSkeleton
